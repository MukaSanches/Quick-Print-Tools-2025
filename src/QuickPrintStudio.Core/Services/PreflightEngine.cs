using System.Collections.Generic;
using QuickPrintStudio.Core.Models;

namespace QuickPrintStudio.Core.Services
{
    public sealed class PreflightEngine
    {
        public PreflightResult Analyze(DocumentSnapshot document)
        {
            var findings = new List<PreflightFinding>();
            if (!document.HasDocument)
            {
                findings.Add(new PreflightFinding("DOC_NONE", "Nenhum documento aberto", "Abra ou crie um documento para iniciar a análise.", FindingSeverity.Info));
                return new PreflightResult(findings);
            }
            if (document.RgbObjectCount > 0)
                findings.Add(new PreflightFinding("COLOR_RGB", $"{document.RgbObjectCount} objeto(s) RGB", "RGB é voltado a telas. Para fluxos de impressão CMYK, revise a conversão antes de produzir.", FindingSeverity.Warning));
            if (document.LowResolutionBitmapCount > 0)
                findings.Add(new PreflightFinding("BITMAP_DPI", $"{document.LowResolutionBitmapCount} imagem(ns) com resolução baixa", "A resolução efetiva depende do tamanho em que a imagem está sendo usada. Revise antes da impressão.", FindingSeverity.Warning));
            if (document.MissingFontCount > 0)
                findings.Add(new PreflightFinding("FONT_MISSING", $"{document.MissingFontCount} fonte(s) precisam de atenção", "Uma fonte ausente ou substituída pode alterar o layout. Localize a fonte ou converta uma cópia do texto em curvas.", FindingSeverity.Error));
            if (document.OutsidePageObjectCount > 0)
                findings.Add(new PreflightFinding("OUTSIDE_PAGE", $"{document.OutsidePageObjectCount} objeto(s) fora da página", "Objetos fora da página podem ser intencionais, mas devem ser revisados antes de fechar o arquivo.", FindingSeverity.Warning));
            if (document.ThinOutlineCount > 0)
                findings.Add(new PreflightFinding("THIN_OUTLINE", $"{document.ThinOutlineCount} contorno(s) muito fino(s)", "Linhas excessivamente finas podem desaparecer ou variar no processo de saída.", FindingSeverity.Warning));
            if (document.TransparencyObjectCount > 0)
                findings.Add(new PreflightFinding("TRANSPARENCY", $"{document.TransparencyObjectCount} objeto(s) com transparência", "Transparências merecem revisão no PDF final, principalmente em fluxos de impressão com RIPs diferentes.", FindingSeverity.Info));
            if (!document.HasBleed)
                findings.Add(new PreflightFinding("BLEED_NONE", "Sangria não detectada", "Sangria é uma sobra da arte além do corte final que ajuda a evitar bordas brancas.", FindingSeverity.Warning));
            if (findings.Count == 0)
                findings.Add(new PreflightFinding("READY", "Nenhum problema básico encontrado", "O documento passou pelas verificações básicas habilitadas. Revise também as exigências específicas da sua produção.", FindingSeverity.Info));
            return new PreflightResult(findings);
        }
    }
}
