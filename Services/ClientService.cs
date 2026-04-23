using FacturationApp.Data;
using FacturationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FacturationApp.Services
{
    public class ClientService
    {
        private readonly AppDbContext _db;
        public ClientService(AppDbContext db) => _db = db;

        public async Task<List<Client>> GetAllAsync(string? search = null)
        {
            var query = _db.Clients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.Contains(search) || (c.Phone != null && c.Phone.Contains(search)));
            return await query.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id) =>
            await _db.Clients.FindAsync(id);

        public async Task AddAsync(Client client)
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            _db.Clients.Update(client);
            await _db.SaveChangesAsync();
        }

        public async Task<string?> DeleteAsync(int id)
        {
            bool hasInvoices = await _db.Invoices.AnyAsync(i => i.ClientId == id);
            if (hasInvoices)
            {
                return "Impossible de supprimer un client ayant des factures";
            }

            var client = await _db.Clients.FindAsync(id);
            if (client != null)
            {
                _db.Clients.Remove(client);
                await _db.SaveChangesAsync();
            }
            return null;
        }
    }
}
