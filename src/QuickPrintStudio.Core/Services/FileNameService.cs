using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace QuickPrintStudio.Core.Services
{
    public static class FileNameService
    {
        public static string ProductionPdf(string? orderId, string? customer, string? product)
        {
            string Clean(string? value)
            {
                var normalized = (value ?? "").Normalize(NormalizationForm.FormD);
                var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).Select(c => char.IsLetterOrDigit(c) ? char.ToUpperInvariant(c) : '_').ToArray();
                var s = new string(chars);
                while (s.Contains("__")) s = s.Replace("__", "_");
                return s.Trim('_');
            }
            var parts = new[] { string.IsNullOrWhiteSpace(orderId) ? "" : "OS-" + Clean(orderId), Clean(customer), Clean(product), "PRODUCAO" }.Where(x => !string.IsNullOrWhiteSpace(x));
            return string.Join("_", parts) + ".pdf";
        }
    }
}
