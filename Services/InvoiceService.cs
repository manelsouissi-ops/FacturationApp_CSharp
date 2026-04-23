using FacturationApp.Data;
using FacturationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FacturationApp.Services
{
    public class InvoiceService
    {
        private readonly AppDbContext _db;
        public InvoiceService(AppDbContext db) => _db = db;

        public async Task<List<Invoice>> GetAllAsync(string? search = null, DateTime? from = null, DateTime? to = null)
        {
            var query = _db.Invoices.Include(i => i.Client).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.Client!.Name.Contains(search) || i.Id.ToString().Contains(search));

            if (from.HasValue)
                query = query.Where(i => i.Date >= from.Value);

            if (to.HasValue)
                query = query.Where(i => i.Date <= to.Value);

            return await query.OrderByDescending(i => i.Date).ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id) =>
            await _db.Invoices
                .Include(i => i.Client)
                .Include(i => i.Lines).ThenInclude(l => l.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<Invoice> CreateAsync(int clientId, List<(int productId, int qty)> items, decimal timbreFiscal)
        {
            var year = DateTime.Now.Year;
            var latestInvoice = await _db.Invoices
                .Where(i => i.Date.Year == year)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (latestInvoice != null && !string.IsNullOrEmpty(latestInvoice.InvoiceNumber))
            {
                var parts = latestInvoice.InvoiceNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            var invoice = new Invoice
            {
                InvoiceNumber = $"FAC-{year}-{nextNumber:D4}",
                Date = DateTime.Now,
                ClientId = clientId,
                TimbreFiscal = timbreFiscal
            };

            foreach (var (productId, qty) in items)
            {
                var product = await _db.Products.FindAsync(productId);
                if (product == null) continue;

                var lineHT = product.PriceHT * qty;
                var lineTVA = lineHT * product.TvaRate / 100;

                invoice.Lines.Add(new InvoiceLine
                {
                    ProductId = productId,
                    Quantity = qty,
                    UnitPriceHT = product.PriceHT,
                    TvaRate = product.TvaRate,
                    TotalHT = lineHT,
                    TotalTVA = lineTVA,
                    TotalTTC = lineHT + lineTVA
                });
            }

            invoice.TotalHT = invoice.Lines.Sum(l => l.TotalHT);
            invoice.TotalTVA = invoice.Lines.Sum(l => l.TotalTVA);
            invoice.TotalTTC = invoice.TotalHT + invoice.TotalTVA + timbreFiscal;

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }

        public async Task DeleteAsync(int id)
        {
            var invoice = await _db.Invoices
                .Include(i => i.Lines)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice != null)
            {
                _db.Invoices.Remove(invoice);
                await _db.SaveChangesAsync();
            }
        }

        // ======================
        // 🔥 ANALYTICS FIXED
        // ======================

        public async Task<decimal> GetTotalTVAAsync() =>
            (decimal)await _db.Invoices.Select(i => (double)i.TotalTVA).SumAsync();

        public async Task<decimal> GetTotalTimbreAsync() =>
            (decimal)await _db.Invoices.Select(i => (double)i.TimbreFiscal).SumAsync();

        public async Task<List<(decimal Rate, decimal Amount)>> GetTVAByRateAsync()
        {
            var data = await _db.InvoiceLines
                .GroupBy(l => l.TvaRate)
                .Select(g => new
                {
                    Rate = (double)g.Key,              // 🔥 FIX
                    Amount = g.Sum(l => (double)l.TotalTVA)
                })
                .OrderBy(x => x.Rate)                 // now double ✅
                .ToListAsync();

            return data
                .Select(x => ((decimal)x.Rate, (decimal)x.Amount))
                .ToList();
        }

        public async Task<decimal> GetCAHTAsync() =>
            (decimal)await _db.Invoices.Select(i => (double)i.TotalHT).SumAsync();

        public async Task<decimal> GetCATTCAsync() =>
            (decimal)await _db.Invoices.Select(i => (double)i.TotalTTC).SumAsync();

        public async Task<List<(string ClientName, decimal HT, decimal TTC)>> GetCAByClientAsync()
        {
            var data = await _db.Invoices
                .Include(i => i.Client)
                .GroupBy(i => i.Client!.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    HT = g.Sum(i => (double)i.TotalHT),
                    TTC = g.Sum(i => (double)i.TotalTTC)
                })
                .OrderByDescending(x => x.TTC)
                .ToListAsync();

            return data.Select(x => (x.Name, (decimal)x.HT, (decimal)x.TTC)).ToList();
        }

        public async Task<List<(string ProductName, decimal HT, decimal TTC)>> GetCAByProductAsync()
        {
            var data = await _db.InvoiceLines
                .Include(l => l.Product)
                .GroupBy(l => l.Product!.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    HT = g.Sum(l => (double)l.TotalHT),
                    TTC = g.Sum(l => (double)l.TotalTTC)
                })
                .OrderByDescending(x => x.TTC)
                .ToListAsync();

            return data.Select(x => (x.Name, (decimal)x.HT, (decimal)x.TTC)).ToList();
        }

        public async Task<List<(string Period, decimal HT, decimal TTC)>> GetCAByMonthAsync()
        {
            var data = await _db.Invoices
                .GroupBy(i => new { i.Date.Year, i.Date.Month })
                .Select(g => new
                {
                    Period = g.Key.Year + "-" + g.Key.Month.ToString("D2"),
                    HT = g.Sum(i => (double)i.TotalHT),
                    TTC = g.Sum(i => (double)i.TotalTTC)
                })
                .ToListAsync(); // 🔥 NO ORDER IN SQL

            // 👉 ORDER IN MEMORY (SAFE)
            return data
                .OrderBy(x => x.Period)
                .Select(x => (x.Period, (decimal)x.HT, (decimal)x.TTC))
                .ToList();
        }
            public async Task<List<Invoice>> GetLastInvoicesAsync(int count)
            {
                return await _db.Invoices
                    .Include(i => i.Client)
                    .OrderByDescending(i => i.Date)
                    .Take(count)
                    .ToListAsync();
            }

            public async Task<(string Name, decimal Amount)?> GetTopClientAsync()
            {
                var top = await _db.Invoices
                    .Include(i => i.Client)
                    .GroupBy(i => i.Client!.Name)
                    .Select(g => new { Name = g.Key, TTC = g.Sum(i => (double)i.TotalTTC) })
                    .OrderByDescending(x => x.TTC)
                    .FirstOrDefaultAsync();

                return top != null ? (top.Name, (decimal)top.TTC) : null;
            }

            public async Task<(string Name, decimal Amount)?> GetTopProductAsync()
            {
                var top = await _db.InvoiceLines
                    .Include(l => l.Product)
                    .GroupBy(l => l.Product!.Name)
                    .Select(g => new { Name = g.Key, TTC = g.Sum(l => (double)l.TotalTTC) })
                    .OrderByDescending(x => x.TTC)
                    .FirstOrDefaultAsync();

                return top != null ? (top.Name, (decimal)top.TTC) : null;
            }

            public async Task<decimal?> GetMonthlyGrowthAsync()
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                var previousDate = DateTime.Now.AddMonths(-1);
                var prevMonth = previousDate.Month;
                var prevYear = previousDate.Year;

                var currentCA = await _db.Invoices
                    .Where(i => i.Date.Month == currentMonth && i.Date.Year == currentYear)
                    .SumAsync(i => (double)i.TotalTTC);

                var prevCA = await _db.Invoices
                    .Where(i => i.Date.Month == prevMonth && i.Date.Year == prevYear)
                    .SumAsync(i => (double)i.TotalTTC);

                if (prevCA == 0) return null;

                return (decimal)((currentCA - prevCA) / prevCA * 100);
            }
        }
    }