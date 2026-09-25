$ErrorActionPreference = "Stop"
$repo = Split-Path -Parent $PSScriptRoot
$possible = @("$env:ProgramFiles\\Corel\\CorelDRAW Graphics Suite 2025\\Programs64", "$env:ProgramFiles\\Corel\\CorelDRAW Graphics Suite 2025\\Programs")
$corel = $possible | Where-Object { Test-Path (Join-Path $_ "CorelDRW.exe") } | Select-Object -First 1
if (-not $corel) { throw "CorelDRAW 2025 não encontrado. Instale o CorelDRAW 2025 para compilar a camada VGCore." }
$vgCandidates = @((Join-Path $corel "Assemblies\\Corel.Interop.VGCore.dll"), (Join-Path $corel "Corel.Interop.VGCore.dll"))
$vg = $vgCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $vg) { throw "Corel.Interop.VGCore.dll não encontrada na instalação detectada: $corel" }
dotnet build "$repo\\src\\QuickPrintStudio.Addon\\QuickPrintStudio.Addon.csproj" -c Release -p:Platform=x64 -p:VGCorePath="$vg"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
$dist = "$repo\\dist\\addon"
Remove-Item $dist -Recurse -Force -ErrorAction SilentlyContinue
New-Item $dist -ItemType Directory -Force | Out-Null
Copy-Item "$repo\\src\\QuickPrintStudio.Addon\\CorelDrw.addon" $dist
Copy-Item "$repo\\src\\QuickPrintStudio.Addon\\AppUI.xslt" $dist
Copy-Item "$repo\\src\\QuickPrintStudio.Addon\\bin\\Release\\net48\\QuickPrintStudio.dll" $dist
Copy-Item "$repo\\src\\QuickPrintStudio.Addon\\bin\\Release\\net48\\QuickPrintStudio.Core.dll" $dist
Write-Host "Addon preparado em $dist"
