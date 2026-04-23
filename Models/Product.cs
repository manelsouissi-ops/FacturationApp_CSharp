using System.ComponentModel.DataAnnotations;

namespace FacturationApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "Le prix doit être positif")]
        public decimal PriceHT { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "100", ErrorMessage = "Le taux TVA doit être entre 0 et 100")]
        public decimal TvaRate { get; set; }

        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    }
}
