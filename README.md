## Introduction
[![CI](https://github.com/wathanz/thermal-image-data-display/actions/workflows/ci.yml/badge.svg)](https://github.com/wathanz/thermal-image-data-display/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/IntensityMapImageViewer.svg)](https://www.nuget.org/packages/IntensityMapImageViewer/)

This project is a data visualization application for 2D array values with pre-defined color mappings. It features a WPF bitmap image user control with zoom/pan and ROI (Region of Interest) overlay support.

Data mapping, ROI drawing, and zoom/pan are fully functional. The projects are actively being updated to:
* Extend test coverage
* Improve code quality

## Demo App (DemoAppNet)
Demonstrates 2D intensity values (thermal data) mapped to RGB image display in a WPF application, using the IntensityMapImageViewer library.

https://github.com/user-attachments/assets/e3cc514c-d3b2-47e6-841b-dc815fd1bf33

## Project List

### Libraries
- IntensityMapping.Core (netstandard2.0)
- WpfCanvasDrawing (net8.0-windows, net48)
- WpfIntensityView (net8.0-windows, net48)

### Sample Application
- IntensityValueGeneration (netstandard2.0)
- DemoAppNet (WPF app, net8.0-windows, net48)

### Test Projects
- ThermalImageDataDisplay.Tests (net8.0-windows)

## Build and Test

**Format the source code:**
```sh
# from the solution directory
dotnet format
```

**Build:**
```sh
# from the solution directory
dotnet build
```

**Test:**
```sh
# from the solution directory
dotnet test
```

### CI quality gates

Pull requests and pushes to `main`/`master` run these checks (Windows, .NET SDK from `global.json` plus the .NET 8 runtime). The final **Quality Gate** job passes only if all of them succeed:

| Job | What it checks |
|-----|----------------|
| Unit Tests | WPF test project; TRX must show executed tests and no failures |
| Core Coverage | `IntensityMapping.Core.Tests` with Coverlet (Cobertura + TRX artifacts, summary in the run). **Report-only** for now; set `CORE_COVERAGE_THRESHOLD` in `ci.yml` (target 80) once the measured baseline is approved |
| Code Analysis | .NET analyzers at `9.0-recommended`. Currently `ANALYZER_MODE: QualityBaseline` (report-only); switch to `QualityGate` after baseline findings are fixed |
| Dependency Audit | NuGet audit (direct + transitive). High/critical (NU1903/NU1904) and audit/source failures fail; lower severities warn |
| Format Check | `dotnet format --verify-no-changes` for the solution and core test project |
| Build and Pack | `build.ps1` build, pack and package checks |

Reproduce locally (PowerShell, from the repository root):
```powershell
New-Item -ItemType Directory -Path build -Force | Out-Null
dotnet build thermal-image-data-display.sln -c Release -p:QualityBaseline=true   # or -p:QualityGate=true
dotnet restore thermal-image-data-display.sln --force --no-cache -p:DependencyAudit=true
dotnet test test/IntensityMapping.Core.Tests -c Release -p:CollectCoverage=true -p:CoverletOutputFormat=cobertura "-p:Include=[IntensityMapping.Core]*"
```

Notes: coverage covers `IntensityMapping.Core` only and does not establish application correctness; NuGet audit is not comprehensive security analysis. Tag-triggered publishing (`release.yml`) is deliberately not gated by these checks. Marking **Quality Gate** as a required check needs a branch protection rule or ruleset in the repository settings.

**Sample video input:**

Copy a small, redistributable MP4 to
`sample\TestAppNet\Media\sample.mp4`. The project copies files from that
folder beside the executable, and the demo loads that bundled video from its
output directory. At startup, choose either the bundled video or the moving
gradient generator. Video frames are converted to grayscale and then rendered
through the selected thermal palette. If the video cannot be opened, the demo
uses the moving gradient generator as a fallback.

```powershell
dotnet run --project sample\TestAppNet\DemoAppNet.csproj -f net8.0-windows
```

## NuGet Package

All libraries under `src/` are packaged into a single NuGet package: **IntensityMapImageViewer**.

**Build and pack (one step):**
```sh
.\build.ps1
```

This will:
1. Build the solution in Release configuration
2. Pack the NuGet package to the `build/` folder
3. Display the package contents for verification

**Pack manually:**
```sh
dotnet build -c Release
nuget pack src\WpfIntensityView\IntensityView.nuspec -OutputDirectory build -Properties packageVersion=1.0.0-alpha
```

The package and assembly versions are defined in `Directory.Build.props`. Update
`VersionPrefix` and `VersionSuffix` there when preparing a release.

The packaging script also runs a smoke test that verifies the expected target
framework assemblies and palette files are present in the generated package.

**Using the package locally:**

The `NuGet.Config` includes a local feed pointing to the `build/` folder. To reference the package in a project:
```xml
<PackageReference Include="IntensityMapImageViewer" Version="1.0.0-alpha" />
```

**Package contents:**

| Assembly | Description |
|----------|-------------|
| IntensityMapping.Core | Shared data model and intensity-to-byte conversion |
| IntensityValueGeneration | Periodic intensity data source for testing |
| WpfCanvasDrawing | ROI graphics drawing overlays |
| WpfIntensityView | WPF image view control with color mapping |

Supported frameworks: `net8.0-windows`, `net48`

## Project Details

### Intensity Mapping (IntensityMapping.Core.csproj)
Shared data model and intensity value conversion to byte value (e.g. 0.0~10.0 to 0~255).

#### Color Mapping Samples
##### Rainbow
![rainbow](image/Rainbow_Palette.bmp)
##### Ironbow
![ironbow](image/Ironbow_Palette.bmp)

### Wpf Canvas Drawing (WpfCanvasDrawing.csproj)
ROI graphics drawing overlays on top of the image view. Canvas drawing code was referenced from https://github.com/songzhu/DrawToolsWPF with necessary changes for ROI rectangle drawing.

### Wpf Intensity View (WpfIntensityView.csproj)
Data is mapped to RGB values and rendered as a bitmap source using the selected color mapping (e.g. rainbow, ironbow).
WPF image view control for displaying data with selected color mapping.

### Intensity Value Generation (IntensityValueGeneration.csproj)
Utility classes that provide periodic intensity data generation for testing.
Can be used as a sample data source instead of real data providers (e.g. sensor/thermal camera).

## Structure
```sh
build.ps1
Directory.Build.props
NuGet.Config
README.md
build/
  IntensityMapImageViewer.*.nupkg
src/
  IntensityMapping.Core/
  IntensityValueGeneration/
  WpfCanvasDrawing/
  WpfIntensityView/
sample/
  DemoAppNet/
test/
  ThermalImageDataDisplay.Tests/
```
