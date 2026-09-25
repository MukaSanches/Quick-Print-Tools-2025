using System;
using System.Collections.Generic;

namespace QuickPrintStudio.Core.Models
{
    public sealed class ProductionPreset
    {
        public string Name { get; set; } = "";
        public double BleedMm { get; set; } = 3;
        public double GapMm { get; set; } = 3;
        public double MarginMm { get; set; } = 5;
        public int MinimumBitmapDpi { get; set; } = 150;
        public bool WarnRgb { get; set; } = true;
        public bool IncludeCropMarks { get; set; } = true;
    }

    public sealed class MeasurementResult
    {
        public double WidthMm { get; set; }
        public double HeightMm { get; set; }
        public double AreaSquareMeters => WidthMm * HeightMm / 1_000_000d;
        public double PerimeterMeters => (WidthMm + HeightMm) * 2d / 1000d;
    }

    public sealed class BatchItem
    {
        public string SourcePath { get; set; } = "";
        public string OutputPath { get; set; } = "";
        public string Status { get; set; } = "Pendente";
    }

    public static class BuiltInPresets
    {
        public static IReadOnlyList<ProductionPreset> All => new[]
        {
            new ProductionPreset { Name="Gráfica geral", BleedMm=3, GapMm=3, MarginMm=5, MinimumBitmapDpi=150 },
            new ProductionPreset { Name="Cartão de visita", BleedMm=3, GapMm=2, MarginMm=5, MinimumBitmapDpi=300 },
            new ProductionPreset { Name="Adesivo", BleedMm=2, GapMm=3, MarginMm=5, MinimumBitmapDpi=150 },
            new ProductionPreset { Name="Banner / grande formato", BleedMm=10, GapMm=10, MarginMm=10, MinimumBitmapDpi=100, IncludeCropMarks=false }
        };
    }
}