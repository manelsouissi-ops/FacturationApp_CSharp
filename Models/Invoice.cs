namespace FacturationApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ClientId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public decimal TimbreFiscal { get; set; }

        public decimal TotalHT { get; set; }

        public decimal TotalTVA { get; set; }

        public decimal TotalTTC { get; set; }

        // 🔥 VERY IMPORTANT (fixes your errors)
        public List<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();

        // Optional (for Include i.Client)
        public Client? Client { get; set; }
    }
}

