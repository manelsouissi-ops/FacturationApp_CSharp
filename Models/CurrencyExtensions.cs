using System.Globalization;

namespace FacturationApp.Models
{
    public static class CurrencyExtensions
    {
        public static string FormatDT(this decimal amount)
        {
            var culture = new CultureInfo("fr-FR"); // French style: space for thousands, comma for decimals
            return string.Format(culture, "{0:#,##0.##} DT", amount);
        }
    }
}
