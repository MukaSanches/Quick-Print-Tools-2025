using System.Linq;
using QuickPrintStudio.Core.Models;
using QuickPrintStudio.Core.Services;
using Xunit;

public sealed class CoreTests
{
 [Fact] public void Imposition_CalculatesPositiveGrid(){ var r=new ImpositionCalculator().Calculate(320,450,90,50,3,5); Assert.True(r.Columns>0 && r.Rows>0 && r.PerSheet==r.Columns*r.Rows); }
 [Fact] public void Imposition_RejectsInvalidDimensions(){ Assert.ThrowsAny<System.ArgumentOutOfRangeException>(()=>new ImpositionCalculator().Calculate(0,450,90,50,3,5)); }
 [Fact] public void Preflight_FindsExpectedProblems(){ var r=new PreflightEngine().Analyze(new DocumentSnapshot{HasDocument=true,RgbObjectCount=2,LowResolutionBitmapCount=1,MissingFontCount=1,HasBleed=false}); Assert.Contains(r.Findings,x=>x.Code=="COLOR_RGB"); Assert.Contains(r.Findings,x=>x.Code=="BITMAP_DPI"); Assert.Contains(r.Findings,x=>x.Code=="FONT_MISSING"); Assert.Contains(r.Findings,x=>x.Code=="BLEED_NONE"); }
 [Fact] public void Preflight_NoDocument_IsSafe(){ var r=new PreflightEngine().Analyze(new DocumentSnapshot{HasDocument=false}); Assert.Single(r.Findings); Assert.Equal("DOC_NONE",r.Findings[0].Code); }
 [Fact] public void Search_UnderstandsBeginnerPhrase(){ var r=new UniversalSearchService().Search("colocar varios cartões na folha").ToList(); Assert.Contains(r,x=>x.Id=="imposition"); }
 [Fact] public void Search_FindsPixQr(){ Assert.Contains(new UniversalSearchService().Search("pix qr"),x=>x.Id=="qr"); }
 [Fact] public void FileName_IsProductionSafe(){ Assert.Equal("OS-2187_JOAO_SAO_PAULO_CARTAO_PRODUCAO.pdf",FileNameService.ProductionPdf("2187","João São Paulo","Cartão")); }
 [Fact] public void Measurement_OneSquareMeter(){ var r=new MeasurementService().FromMillimeters(1000,1000); Assert.Equal(1,r.AreaSquareMeters,6); Assert.Equal(4,r.PerimeterMeters,6); }
 [Fact] public void StepRepeat_ReturnsEveryPosition(){ var p=new StepRepeatService().Calculate(3,2,90,50,3,3); Assert.Equal(6,p.Count); Assert.Equal((186d,53d),p.Last()); }
 [Fact] public void BuiltInPresets_HaveBeginnerAndLargeFormatOptions(){ Assert.Contains(BuiltInPresets.All,x=>x.Name=="Cartão de visita"); Assert.Contains(BuiltInPresets.All,x=>x.Name.Contains("grande formato")); }
}
