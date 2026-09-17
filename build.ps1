# Build and pack IntensityMapImageViewer NuGet package
param(
    [string]$Configuration = "Release",
    [string]$OutputDir = "$PSScriptRoot\build",
    [string]$BuildNumber = "0"
)

$ErrorActionPreference = "Stop"

# Ensure output directory exists (needed for NuGet.Config local feed)
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

$versionProps = "-p:BuildNumber=$BuildNumber"

Write-Host "=== Building src projects ($Configuration) ===" -ForegroundColor Cyan
dotnet build "$PSScriptRoot\src\IntensityMapping.Core\IntensityMapping.Core.csproj" -c $Configuration $versionProps
if ($LASTEXITCODE -ne 0) { throw "IntensityMapping.Core build failed" }
dotnet build "$PSScriptRoot\src\WpfCanvasDrawing\WpfCanvasDrawing.csproj" -c $Configuration $versionProps
if ($LASTEXITCODE -ne 0) { throw "WpfCanvasDrawing build failed" }
dotnet build "$PSScriptRoot\src\WpfIntensityView\WpfIntensityView.csproj" -c $Configuration $versionProps
if ($LASTEXITCODE -ne 0) { throw "WpfIntensityView build failed" }
dotnet build "$PSScriptRoot\src\IntensityValueGeneration\IntensityValueGeneration.csproj" -c $Configuration $versionProps
if ($LASTEXITCODE -ne 0) { throw "IntensityValueGeneration build failed" }

Write-Host "`n=== Packing NuGet package ===" -ForegroundColor Cyan

$packageVersion = (dotnet msbuild "$PSScriptRoot\src\WpfIntensityView\WpfIntensityView.csproj" -getProperty:PackageVersion $versionProps -nologo | Select-Object -Last 1).Trim()
if ([string]::IsNullOrWhiteSpace($packageVersion)) { throw "Unable to determine package version" }
Write-Host "Package version: $packageVersion"

# Ensure NuGet CLI is available (required for 'nuget pack')
$nugetCmd = Get-Command nuget -ErrorAction SilentlyContinue
if (-not $nugetCmd) {
    Write-Host "NuGet CLI not found on PATH. Installing NuGet.CommandLine as a dotnet global tool..." -ForegroundColor Yellow
    dotnet tool install -g NuGet.CommandLine
    if ($LASTEXITCODE -ne 0) { throw "Failed to install NuGet.CommandLine global tool" }

    # Add default dotnet tools path for the current session (Windows default)
    $dotnetToolsPath = Join-Path $env:USERPROFILE ".dotnet\tools"
    if (Test-Path $dotnetToolsPath -and ($env:PATH -notlike "*$dotnetToolsPath*")) {
        $env:PATH = "$dotnetToolsPath;$env:PATH"
    }
}
nuget pack "$PSScriptRoot\src\WpfIntensityView\IntensityView.nuspec" -OutputDirectory $OutputDir -Properties "packageVersion=$packageVersion;configuration=$Configuration"
if ($LASTEXITCODE -ne 0) { throw "Pack failed" }

Write-Host "`n=== Building full solution ===" -ForegroundColor Cyan
dotnet build "$PSScriptRoot\thermal-image-data-display.sln" -c $Configuration $versionProps
if ($LASTEXITCODE -ne 0) { throw "Build failed" }

# Verify package contents
$nupkg = Get-ChildItem $OutputDir -Filter "IntensityMapImageViewer.*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $nupkg) {
    throw "No IntensityMapImageViewer package found in $OutputDir"
}

Write-Host "`n=== Package contents ===" -ForegroundColor Cyan
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead($nupkg.FullName)
try {
        $zip.Entries | Where-Object { $_.FullName -match '\.(dll|bmp|xml)$' } | Sort-Object FullName | ForEach-Object { Write-Host "  $_" }

        $expectedEntries = @(
            "lib/net48/IntensityMapping.Core.dll",
            "lib/net48/IntensityValueGeneration.dll",
            "lib/net48/WpfCanvasDrawing.dll",
            "lib/net48/WpfIntensityView.dll",
            "lib/net8.0-windows7.0/IntensityMapping.Core.dll",
            "lib/net8.0-windows7.0/IntensityValueGeneration.dll",
            "lib/net8.0-windows7.0/WpfCanvasDrawing.dll",
            "lib/net8.0-windows7.0/WpfIntensityView.dll",
            "contentFiles/any/any/ReferenceImage/Blackhot_Palette.bmp",
            "contentFiles/any/any/ReferenceImage/Ironbow_Palette.bmp",
            "contentFiles/any/any/ReferenceImage/Rainbow_Palette.bmp",
            "contentFiles/any/any/ReferenceImage/RainbowFlip_Palette.bmp",
            "contentFiles/any/any/ReferenceImage/Whitehot_Palette.bmp"
        )
        $actualEntries = @($zip.Entries | ForEach-Object { $_.FullName })
        $missingEntries = @($expectedEntries | Where-Object { $_ -notin $actualEntries })
        if ($missingEntries.Count -gt 0) {
            throw "Package is missing expected entries: $($missingEntries -join ', ')"
        }
        Write-Host "Package smoke test passed: $($expectedEntries.Count) expected entries found" -ForegroundColor Green
    }
finally {
    $zip.Dispose()
}
Write-Host "`nPackage: $($nupkg.FullName)" -ForegroundColor Green
