using System;

namespace QuickPrintStudio.Core.Services
{
    public sealed class ImpositionResult
    {
        public int Columns { get; set; }
        public int Rows { get; set; }
        public int PerSheet => Columns * Rows;
    }

    public sealed class ImpositionCalculator
    {
        public ImpositionResult Calculate(double sheetWidthMm, double sheetHeightMm, double itemWidthMm, double itemHeightMm, double gapMm, double marginMm)
        {
            if (sheetWidthMm <= 0 || sheetHeightMm <= 0 || itemWidthMm <= 0 || itemHeightMm <= 0) throw new ArgumentOutOfRangeException("As dimensões devem ser maiores que zero.");
            if (gapMm < 0 || marginMm < 0) throw new ArgumentOutOfRangeException("Espaçamento e margem não podem ser negativos.");
            var usableWidth = Math.Max(0, sheetWidthMm - 2 * marginMm);
            var usableHeight = Math.Max(0, sheetHeightMm - 2 * marginMm);
            int columns = (int)Math.Floor((usableWidth + gapMm) / (itemWidthMm + gapMm));
            int rows = (int)Math.Floor((usableHeight + gapMm) / (itemHeightMm + gapMm));
            return new ImpositionResult { Columns = Math.Max(0, columns), Rows = Math.Max(0, rows) };
        }
    }
}
