using FacturationApp.Data;
using FacturationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FacturationApp.Services
{
    public class ProductService
    {
        private readonly AppDbContext _db;
        public ProductService(AppDbContext db) => _db = db;

        public async Task<List<Product>> GetAllAsync(string? search = null)
        {
            var query = _db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));
            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id) =>
            await _db.Products.FindAsync(id);

        public async Task AddAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            var existing = await _db.Products.FindAsync(product.Id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.PriceHT = product.PriceHT;
                existing.TvaRate = product.TvaRate;

                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            bool hasInvoiceLines = await _db.InvoiceLines.AnyAsync(il => il.ProductId == id);
            if (hasInvoiceLines)
            {
                throw new InvalidOperationException("Impossible de supprimer un produit utilisé dans des factures");
            }

            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }
        }
    }
}
