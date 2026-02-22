# Build and pack IntensityMapImageViewer NuGet package
param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "$PSScriptRoot\build"
)

$ErrorActionPreference = "Stop"

# Ensure output directory exists (needed for NuGet.Config local feed)
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host "=== Building src projects ($Configuration) ===" -ForegroundColor Cyan
dotnet build "$PSScriptRoot\src\IntensityMapping.Core" -c $Configuration
dotnet build "$PSScriptRoot\src\WpfCanvasDrawing" -c $Configuration
dotnet build "$PSScriptRoot\src\WpfIntensityView" -c $Configuration
dotnet build "$PSScriptRoot\src\IntensityValueGeneration" -c $Configuration
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

Write-Host "`n=== Packing NuGet package ===" -ForegroundColor Cyan
nuget pack "$PSScriptRoot\src\WpfIntensityView\IntensityView.nuspec" -OutputDirectory $OutputDir
if ($LASTEXITCODE -ne 0) { throw "Pack failed" }

Write-Host "`n=== Building full solution ===" -ForegroundColor Cyan
dotnet build "$PSScriptRoot\thermal-image-data-display.sln" -c $Configuration
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

# Verify package contents
$nupkg = Get-ChildItem $OutputDir -Filter "IntensityMapImageViewer.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($nupkg) {
    Write-Host "`n=== Package contents ===" -ForegroundColor Cyan
    $zip = [System.IO.Compression.ZipFile]::OpenRead($nupkg.FullName)
    $zip.Entries | Where-Object { $_.FullName -match '\.(dll|bmp|xml)$' } | Sort-Object FullName | ForEach-Object { Write-Host "  $_" }
    $zip.Dispose()
    Write-Host "`nPackage: $($nupkg.FullName)" -ForegroundColor Green
}
