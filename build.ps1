# Build and pack IntensityMapImageViewer NuGet package
param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "$PSScriptRoot\build"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Building solution ($Configuration) ===" -ForegroundColor Cyan
dotnet build "$PSScriptRoot\thermal-image-data-display.sln" -c $Configuration
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

Write-Host "`n=== Packing NuGet package ===" -ForegroundColor Cyan
nuget pack "$PSScriptRoot\src\WpfIntensityView\IntensityView.nuspec" -OutputDirectory $OutputDir
if ($LASTEXITCODE -ne 0) { throw "Pack failed" }

# Verify package contents
$nupkg = Get-ChildItem $OutputDir -Filter "IntensityMapImageViewer.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($nupkg) {
    Write-Host "`n=== Package contents ===" -ForegroundColor Cyan
    $zip = [System.IO.Compression.ZipFile]::OpenRead($nupkg.FullName)
    $zip.Entries | Where-Object { $_.FullName -match '\.(dll|bmp|xml)$' } | Sort-Object FullName | ForEach-Object { Write-Host "  $_" }
    $zip.Dispose()
    Write-Host "`nPackage: $($nupkg.FullName)" -ForegroundColor Green
}
