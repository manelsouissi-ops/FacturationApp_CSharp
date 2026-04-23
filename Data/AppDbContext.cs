using FacturationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FacturationApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Client)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.ClientId);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<InvoiceLine>()
                .HasOne(l => l.Invoice)
                .WithMany(i => i.Lines)
                .HasForeignKey(l => l.InvoiceId);

            modelBuilder.Entity<InvoiceLine>()
                .HasOne(l => l.Product)
                .WithMany(p => p.InvoiceLines)
                .HasForeignKey(l => l.ProductId);

            // Seed data
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Ahmed Ben Ali", Phone = "+216 71 123 456", Address = "Tunis, Tunisie" },
                new Client { Id = 2, Name = "Société XYZ", Phone = "+216 73 456 789", Address = "Sfax, Tunisie" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Ordinateur Portable", PriceHT = 1500m, TvaRate = 19m },
                new Product { Id = 2, Name = "Souris USB", PriceHT = 25m, TvaRate = 19m },
                new Product { Id = 3, Name = "Clavier", PriceHT = 45m, TvaRate = 19m },
                new Product { Id = 4, Name = "Écran 24\"", PriceHT = 350m, TvaRate = 7m }
            );
        }
    }
}
