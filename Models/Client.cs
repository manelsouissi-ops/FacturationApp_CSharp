using System.ComponentModel.DataAnnotations;

namespace FacturationApp.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        public string? Phone { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
