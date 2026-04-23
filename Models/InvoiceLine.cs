using System.ComponentModel.DataAnnotations;

namespace FacturationApp.Models
{
    public class InvoiceLine
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être au moins 1")]
        public int Quantity { get; set; } = 1;

        public decimal UnitPriceHT { get; set; }
        public decimal TvaRate { get; set; }
        public decimal TotalHT { get; set; }
        public decimal TotalTVA { get; set; }
        public decimal TotalTTC { get; set; }

        // Navigation
        public Invoice? Invoice { get; set; }
        public Product? Product { get; set; }
    }
}
