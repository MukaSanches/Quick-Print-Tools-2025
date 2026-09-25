using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuickPrintStudio.Core.Models;
using QuickPrintStudio.Core.Services;

namespace QuickPrintStudio.Addon.Views
{
    public partial class DockerView : UserControl
    {
        private readonly PreflightEngine _preflight = new PreflightEngine();
        private readonly UniversalSearchService _search = new UniversalSearchService();
        public DockerView(){ InitializeComponent(); RefreshDocument(); RenderSearch(""); }
        private void RefreshDocument(){ DocumentNameText.Text = "Quick Print Studio carregado"; DocumentMetaText.Text = "A integração VGCore será ativada no build feito em uma máquina com CorelDRAW 2025."; }
        private DocumentSnapshot CaptureSnapshot(){ return new DocumentSnapshot { HasDocument = false }; }
        private void Analyze_OnClick(object sender, RoutedEventArgs e){ var result=_preflight.Analyze(CaptureSnapshot()); ResultsList.ItemsSource=result.Findings.Select(f => $"{(f.Severity==FindingSeverity.Error?"ERRO":f.Severity==FindingSeverity.Warning?"ATENÇÃO":"INFO")} — {f.Title}\n{f.Explanation}"); }
        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e){ RenderSearch(SearchBox.Text); }
        private void RenderSearch(string query){ SearchResults.ItemsSource=_search.Search(query).Take(8).Select(t => $"{t.Title}\n{t.Description}"); }
    }
}
