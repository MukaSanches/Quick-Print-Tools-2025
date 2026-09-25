using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickPrintStudio.Core.Services
{
    public sealed class ToolDescriptor
    {
        public string Id { get; }
        public string Title { get; }
        public string Description { get; }
        public string[] Keywords { get; }
        public ToolDescriptor(string id, string title, string description, params string[] keywords) { Id=id; Title=title; Description=description; Keywords=keywords; }
    }

    public sealed class UniversalSearchService
    {
        private readonly IReadOnlyList<ToolDescriptor> _tools = new[]
        {
            new ToolDescriptor("preflight", "Analisar documento", "Verifica problemas comuns antes da produção.", "preflight","analisar","erro","problema","arquivo"),
            new ToolDescriptor("bleed", "Sangria", "Prepara a sobra da arte para o corte.", "sangria","borda","corte","3mm"),
            new ToolDescriptor("cropmarks", "Marcas de corte", "Cria marcas que indicam onde o material deve ser cortado.", "marca","corte","trim"),
            new ToolDescriptor("imposition", "Imposição", "Organiza várias cópias na folha para aproveitar material.", "imposição","cartao","cartão","folha","sra3","repetir","varios","vários"),
            new ToolDescriptor("qr", "QR Code", "Cria QR para PIX, WhatsApp, URL, texto e outros usos.", "qr","pix","whatsapp","url"),
            new ToolDescriptor("measure", "Medidas", "Mostra tamanho, área e perímetro da seleção.", "medir","area","área","perimetro","perímetro","banner","m2"),
            new ToolDescriptor("export", "Exportar para produção", "Exporta usando presets voltados à produção gráfica.", "pdf","exportar","produção","png","jpg")
        };

        public IEnumerable<ToolDescriptor> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return _tools;
            var terms = query.ToLowerInvariant().Split(new[] {' ', '-', '_'}, StringSplitOptions.RemoveEmptyEntries);
            return _tools.Select(t => new { Tool=t, Score=terms.Count(term => t.Title.ToLowerInvariant().Contains(term) || t.Description.ToLowerInvariant().Contains(term) || t.Keywords.Any(k => k.Contains(term))) })
                .Where(x => x.Score > 0).OrderByDescending(x => x.Score).ThenBy(x => x.Tool.Title).Select(x => x.Tool);
        }
    }
}
