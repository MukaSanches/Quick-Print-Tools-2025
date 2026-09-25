$ErrorActionPreference = "Stop"
$repo = Split-Path -Parent $PSScriptRoot
$version = (Get-Content (Join-Path $repo "VERSION") -Raw).Trim()
if ($version -ne "1.0.0") { throw "Versao inesperada: $version" }

Write-Host "Quick Print Studio $version - validacao e build"
dotnet test "$repo\tests\QuickPrintStudio.Core.Tests\QuickPrintStudio.Core.Tests.csproj" -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

[xml](Get-Content "$repo\src\QuickPrintStudio.Addon\AppUI.xslt" -Raw) | Out-Null
[xml](Get-Content "$repo\src\QuickPrintStudio.Addon\Views\DockerView.xaml" -Raw) | Out-Null

$possible = @(
  "$env:ProgramFiles\Corel\CorelDRAW Graphics Suite 2025\Programs64",
  "$env:ProgramFiles\Corel\CorelDRAW Graphics Suite 2025\Programs"
)
$corel = $possible | Where-Object { Test-Path (Join-Path $_ "CorelDRW.exe") } | Select-Object -First 1
if (-not $corel) { throw "CorelDRAW 2025 nao encontrado. O Core testou corretamente, mas o addon real exige CorelDRAW 2025 para obter VGCore." }

$vgCandidates = @(
  (Join-Path $corel "Assemblies\Corel.Interop.VGCore.dll"),
  (Join-Path $corel "Corel.Interop.VGCore.dll")
)
$vg = $vgCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $vg) { throw "Corel.Interop.VGCore.dll nao encontrada na instalacao detectada: $corel" }

dotnet build "$repo\src\QuickPrintStudio.Addon\QuickPrintStudio.Addon.csproj" -c Release -p:Platform=x64 -p:VGCorePath="$vg"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$dist = "$repo\dist\addon"
Remove-Item $dist -Recurse -Force -ErrorAction SilentlyContinue
New-Item $dist -ItemType Directory -Force | Out-Null
Copy-Item "$repo\src\QuickPrintStudio.Addon\CorelDrw.addon" $dist
Copy-Item "$repo\src\QuickPrintStudio.Addon\AppUI.xslt" $dist
Copy-Item "$repo\src\QuickPrintStudio.Addon\bin\Release\net48\QuickPrintStudio.dll" $dist
Copy-Item "$repo\src\QuickPrintStudio.Addon\bin\Release\net48\QuickPrintStudio.Core.dll" $dist
Copy-Item "$repo\VERSION" $dist
Write-Host "Addon $version preparado em $dist"

$makensis = @(
 "$env:ProgramFiles(x86)\NSIS\makensis.exe",
 "$env:ProgramFiles\NSIS\makensis.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if ($makensis) {
  & $makensis "$repo\installer\QuickPrintStudio.nsi"
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
  Write-Host "Instalador gerado."
} else {
  Write-Warning "NSIS nao encontrado. O addon foi compilado, mas o EXE do instalador nao foi gerado."
}
