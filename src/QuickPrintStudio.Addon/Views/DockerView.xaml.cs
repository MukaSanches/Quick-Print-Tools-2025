using System;
using System.Globalization;
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
        private readonly ImpositionCalculator _imposition = new ImpositionCalculator();
        private readonly MeasurementService _measurement = new MeasurementService();

        public DockerView()
        {
            InitializeComponent();
            PresetBox.ItemsSource = BuiltInPresets.All.Select(x => x.Name);
            PresetBox.SelectedIndex = 0;
            RefreshDocument();
            RenderSearch("");
        }

        private void RefreshDocument()
        {
            DocumentNameText.Text = "Quick Print Studio 1.0";
            DocumentMetaText.Text = "Painel carregado. Recursos independentes estão disponíveis; ações VGCore ficam protegidas até a integração real ser validada.";
        }

        private DocumentSnapshot CaptureSnapshot()
        {
            // O adapter VGCore v26 substitui este snapshot seguro em builds integrados ao CorelDRAW.
            // Não inventamos dados do documento quando o host Corel não está conectado.
            return new DocumentSnapshot { HasDocument = false };
        }

        private void Analyze_OnClick(object sender, RoutedEventArgs e)
        {
            var result = _preflight.Analyze(CaptureSnapshot());
            ResultsList.ItemsSource = result.Findings.Select(f =>
                $"{(f.Severity == FindingSeverity.Error ? "ERRO" : f.Severity == FindingSeverity.Warning ? "ATENÇÃO" : "INFO")} — {f.Title}\n{f.Explanation}");
            MainTabs.SelectedIndex = 1;
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e) => RenderSearch(SearchBox.Text);
        private void RenderSearch(string query) => SearchResults.ItemsSource = _search.Search(query).Take(8).Select(t => $"{t.Title}\n{t.Description}");

        private void OpenProduction_OnClick(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 2;
        private void OpenImposition_OnClick(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 3;

        private static double Parse(TextBox box)
        {
            if (double.TryParse(box.Text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) return value;
            throw new FormatException("Digite apenas números válidos.");
        }

        private void CalculateImposition_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var r = _imposition.Calculate(Parse(SheetW), Parse(SheetH), Parse(ItemW), Parse(ItemH), Parse(Gap), Parse(Margin));
                ImpositionResultText.Text = $"Resultado: {r.Columns} coluna(s) × {r.Rows} linha(s) = {r.PerSheet} peça(s) por folha.";
            }
            catch (Exception ex) { ImpositionResultText.Text = "Não foi possível calcular: " + ex.Message; }
        }

        private void CalculateMeasure_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var r = _measurement.FromMillimeters(Parse(MeasureW), Parse(MeasureH));
                MeasureResultText.Text = $"Área: {r.AreaSquareMeters:0.###} m² • Perímetro: {r.PerimeterMeters:0.###} m";
            }
            catch (Exception ex) { MeasureResultText.Text = "Não foi possível calcular: " + ex.Message; }
        }

        private void PreviewFileName_OnClick(object sender, RoutedEventArgs e)
        {
            FileNameText.Text = FileNameService.ProductionPdf(OrderId.Text, Customer.Text, Product.Text);
        }
    }
}
