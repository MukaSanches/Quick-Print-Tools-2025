using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
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
        private dynamic _corel;

        public DockerView()
        {
            InitializeComponent();
            PresetBox.ItemsSource = BuiltInPresets.All.Select(x => x.Name);
            PresetBox.SelectedIndex = 0;
            TryConnectCorel();
            RefreshDocument();
            RenderSearch("");
        }

        private void TryConnectCorel()
        {
            try { _corel = Marshal.GetActiveObject("CorelDRAW.Application.26"); }
            catch { _corel = null; }
        }

        private void RefreshDocument()
        {
            TryConnectCorel();
            try
            {
                if (_corel != null && _corel.Documents.Count > 0)
                {
                    dynamic d = _corel.ActiveDocument;
                    DocumentNameText.Text = Convert.ToString(d.Name);
                    DocumentMetaText.Text = $"CorelDRAW conectado • {d.Pages.Count} página(s) • análise do documento real disponível.";
                    return;
                }
            }
            catch { }
            DocumentNameText.Text = "Quick Print Studio 1.3";
            DocumentMetaText.Text = _corel == null ? "CorelDRAW ainda não conectado. Abra o CorelDRAW 2025 e um documento." : "CorelDRAW conectado. Abra um documento para iniciar.";
        }

        private DocumentSnapshot CaptureSnapshot()
        {
            TryConnectCorel();
            if (_corel == null) return new DocumentSnapshot { HasDocument = false };
            try
            {
                if (_corel.Documents.Count == 0) return new DocumentSnapshot { HasDocument = false };
                dynamic d = _corel.ActiveDocument;
                dynamic p = d.ActivePage;
                int shapes = 0, rgb = 0, low = 0, transparency = 0, thin = 0, outside = 0;
                foreach (dynamic s in p.Shapes)
                {
                    shapes++;
                    try { if (Convert.ToInt32(s.Fill.UniformColor.Type) == 5) rgb++; } catch { }
                    try
                    {
                        if (s.Type == 7)
                        {
                            dynamic b = s.Bitmap;
                            double dpi = Math.Min(Convert.ToDouble(b.ResolutionX), Convert.ToDouble(b.ResolutionY));
                            if (dpi > 0 && dpi < 150) low++;
                        }
                    } catch { }
                    try { if (Convert.ToInt32(s.Transparency.Type) != 0) transparency++; } catch { }
                    try { if (s.Outline.Type != 0 && Convert.ToDouble(s.Outline.Width) > 0 && Convert.ToDouble(s.Outline.Width) < 0.1) thin++; } catch { }
                    try
                    {
                        double left = Convert.ToDouble(s.LeftX), right = Convert.ToDouble(s.RightX);
                        double top = Convert.ToDouble(s.TopY), bottom = Convert.ToDouble(s.BottomY);
                        if (left < 0 || bottom < 0 || right > Convert.ToDouble(p.SizeWidth) || top > Convert.ToDouble(p.SizeHeight)) outside++;
                    } catch { }
                }
                return new DocumentSnapshot {
                    HasDocument=true, Name=Convert.ToString(d.Name), PageCount=Convert.ToInt32(d.Pages.Count),
                    WidthMm=Convert.ToDouble(p.SizeWidth), HeightMm=Convert.ToDouble(p.SizeHeight), ShapeCount=shapes,
                    RgbObjectCount=rgb, LowResolutionBitmapCount=low, TransparencyObjectCount=transparency,
                    ThinOutlineCount=thin, OutsidePageObjectCount=outside, HasBleed=false
                };
            }
            catch { return new DocumentSnapshot { HasDocument = false }; }
        }

        private void Analyze_OnClick(object sender, RoutedEventArgs e)
        {
            var snapshot = CaptureSnapshot();
            var result = _preflight.Analyze(snapshot);
            ResultsList.ItemsSource = result.Findings.Select(f => $"{(f.Severity == FindingSeverity.Error ? "ERRO" : f.Severity == FindingSeverity.Warning ? "ATENÇÃO" : "INFO")} — {f.Title}\n{f.Explanation}");
            if (snapshot.HasDocument) DocumentMetaText.Text = $"{snapshot.PageCount} pág. • {snapshot.ShapeCount} objeto(s) • {result.ErrorCount} erro(s) • {result.WarningCount} alerta(s)";
            MainTabs.SelectedIndex = 1;
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e) => RenderSearch(SearchBox.Text);
        private void RenderSearch(string query) => SearchResults.ItemsSource = _search.Search(query).Take(8).Select(t => $"{t.Title}\n{t.Description}");
        private void OpenProduction_OnClick(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 2;
        private void OpenImposition_OnClick(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 3;
        private static double Parse(TextBox box) { if (double.TryParse(box.Text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) return value; throw new FormatException("Digite apenas números válidos."); }

        private void CalculateImposition_OnClick(object sender, RoutedEventArgs e)
        {
            try { var r=_imposition.Calculate(Parse(SheetW),Parse(SheetH),Parse(ItemW),Parse(ItemH),Parse(Gap),Parse(Margin)); ImpositionResultText.Text=$"Resultado: {r.Columns} coluna(s) × {r.Rows} linha(s) = {r.PerSheet} peça(s) por folha."; }
            catch(Exception ex){ ImpositionResultText.Text="Não foi possível calcular: "+ex.Message; }
        }

        private void CalculateMeasure_OnClick(object sender, RoutedEventArgs e)
        {
            try { var r=_measurement.FromMillimeters(Parse(MeasureW),Parse(MeasureH)); MeasureResultText.Text=$"Área: {r.AreaSquareMeters:0.###} m² • Perímetro: {r.PerimeterMeters:0.###} m"; }
            catch(Exception ex){ MeasureResultText.Text="Não foi possível calcular: "+ex.Message; }
        }

        private void PreviewFileName_OnClick(object sender, RoutedEventArgs e) { FileNameText.Text=FileNameService.ProductionPdf(OrderId.Text,Customer.Text,Product.Text); }

        private void PublishPdf_OnClick(object sender, RoutedEventArgs e)
        {
            TryConnectCorel();
            if (_corel == null || _corel.Documents.Count == 0) { MessageBox.Show("Abra um documento no CorelDRAW 2025 primeiro."); return; }
            var name=FileNameService.ProductionPdf(OrderId.Text,Customer.Text,Product.Text);
            var dlg=new SaveFileDialog { Filter="PDF (*.pdf)|*.pdf", FileName=name, AddExtension=true };
            if (dlg.ShowDialog()!=true) return;
            try { dynamic d=_corel.ActiveDocument; d.PublishToPDF(dlg.FileName); MessageBox.Show("PDF criado com sucesso:\n"+dlg.FileName,"Quick Print Studio"); }
            catch(Exception ex){ MessageBox.Show("O CorelDRAW não conseguiu publicar o PDF.\n"+ex.Message,"Quick Print Studio"); }
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e) { RefreshDocument(); }
    }
}