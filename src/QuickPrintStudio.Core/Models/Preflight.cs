using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickPrintStudio.Core.Models
{
    public enum FindingSeverity { Info, Warning, Error }

    public sealed class PreflightFinding
    {
        public string Code { get; }
        public string Title { get; }
        public string Explanation { get; }
        public FindingSeverity Severity { get; }
        public string? ObjectId { get; }

        public PreflightFinding(string code, string title, string explanation, FindingSeverity severity, string? objectId = null)
        {
            Code = code; Title = title; Explanation = explanation; Severity = severity; ObjectId = objectId;
        }
    }

    public sealed class DocumentSnapshot
    {
        public string Name { get; set; } = "Sem documento";
        public double WidthMm { get; set; }
        public double HeightMm { get; set; }
        public int PageCount { get; set; }
        public int ShapeCount { get; set; }
        public int RgbObjectCount { get; set; }
        public int LowResolutionBitmapCount { get; set; }
        public int MissingFontCount { get; set; }
        public int OutsidePageObjectCount { get; set; }
        public int ThinOutlineCount { get; set; }
        public int TransparencyObjectCount { get; set; }
        public int TextObjectCount { get; set; }
        public double MinimumBitmapDpi { get; set; } = 300;
        public bool HasBleed { get; set; }
        public bool HasDocument { get; set; }
    }

    public sealed class PreflightResult
    {
        public IReadOnlyList<PreflightFinding> Findings { get; }
        public int ErrorCount => Findings.Count(x => x.Severity == FindingSeverity.Error);
        public int WarningCount => Findings.Count(x => x.Severity == FindingSeverity.Warning);
        public int InfoCount => Findings.Count(x => x.Severity == FindingSeverity.Info);
        public bool IsReady => ErrorCount == 0;
        public PreflightResult(IEnumerable<PreflightFinding> findings) { Findings = findings.ToArray(); }
    }
}
