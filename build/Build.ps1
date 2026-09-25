$ErrorActionPreference = "Stop"
$repo = Split-Path -Parent $PSScriptRoot
$version = (Get-Content (Join-Path $repo "VERSION") -Raw).Trim()
Write-Host "Quick Print Studio $version - portable build (sem VGCore no build)"

dotnet test "$repo\tests\QuickPrintStudio.Core.Tests\QuickPrintStudio.Core.Tests.csproj" -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

[xml](Get-Content "$repo\src\QuickPrintStudio.Addon\AppUI.xslt" -Raw) | Out-Null
[xml](Get-Content "$repo\src\QuickPrintStudio.Addon\Views\DockerView.xaml" -Raw) | Out-Null

dotnet build "$repo\src\QuickPrintStudio.Addon\QuickPrintStudio.Addon.csproj" -c Release -p:Platform=x64
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$dist = "$repo\dist\addon"
Remove-Item $dist -Recurse -Force -ErrorAction SilentlyContinue
New-Item $dist -ItemType Directory -Force | Out-Null
Copy-Item "$repo\src\QuickPrintStudio.Addon\CorelDrw.addon" $dist
Copy-Item "$repo\src\QuickPrintStudio.Addon\AppUI.xslt" $dist
Copy-Item "$repo\src\QuickPrintStudio.Addon\bin\Release\net48\QuickPrintStudio.dll" $dist
Copy-Item "$repo\src\QuickPrintStudio.Addon\bin\Release\net48\QuickPrintStudio.Core.dll" $dist
Copy-Item "$repo\VERSION" $dist

$makensis = @(
 "$env:ProgramFiles(x86)\NSIS\makensis.exe",
 "$env:ProgramFiles\NSIS\makensis.exe"
) | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $makensis) { throw "NSIS nao encontrado." }

Push-Location "$repo\installer"
try {
  & $makensis "QuickPrintStudio.nsi"
  if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
} finally { Pop-Location }

$exe = "$repo\installer\QuickPrintStudio-$version-Setup.exe"
if (-not (Test-Path $exe)) { throw "Instalador nao foi gerado: $exe" }
Write-Host "Instalador gerado: $exe"
