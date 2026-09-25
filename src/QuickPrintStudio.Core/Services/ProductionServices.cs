using System;
using System.Collections.Generic;
using System.Linq;
using QuickPrintStudio.Core.Models;

namespace QuickPrintStudio.Core.Services
{
    public sealed class MeasurementService
    {
        public MeasurementResult FromMillimeters(double width, double height)
        {
            if (width < 0 || height < 0) throw new ArgumentOutOfRangeException();
            return new MeasurementResult { WidthMm=width, HeightMm=height };
        }
    }

    public sealed class StepRepeatService
    {
        public IReadOnlyList<(double X, double Y)> Calculate(int columns, int rows, double itemWidth, double itemHeight, double gapX, double gapY)
        {
            if (columns < 1 || rows < 1) throw new ArgumentOutOfRangeException();
            var points = new List<(double,double)>(columns*rows);
            for (var row=0; row<rows; row++)
                for (var col=0; col<columns; col++)
                    points.Add((col*(itemWidth+gapX), row*(itemHeight+gapY)));
            return points;
        }
    }

    public sealed class PreflightSummaryService
    {
        public string Explain(PreflightResult result)
        {
            if (result.ErrorCount > 0) return $"Encontramos {result.ErrorCount} problema(s) que podem impedir uma produção segura e {result.WarningCount} aviso(s).";
            if (result.WarningCount > 0) return $"Não há erros críticos, mas existem {result.WarningCount} ponto(s) para revisar antes de produzir.";
            return "Nenhum erro crítico foi encontrado pelas verificações habilitadas.";
        }
    }
}