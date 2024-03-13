## Introduction
This project is a data visualization application for 2D array values with pre-defined color mappings. It features a WPF bitmap image user control with zoom/pan and ROI (Region of Interest) overlay support.

Data mapping, ROI drawing, and zoom/pan are fully functional. The projects are actively being updated to:
* Extend test coverage
* Improve code quality

## Project List

### Libraries
- IntensityMapping.Core (netstandard2.0)
- WpfCanvasDrawing (net8.0-windows, net48)
- WpfIntensityView (net8.0-windows, net48)

### Sample Application
- IntensityValueGeneration (netstandard2.0)
- DemoAppNet (WPF app, net8.0-windows, net48)

### Test Projects
- IntensityMapping.Core.Tests (net8.0)

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


### Intensity Mapping (IntensityMapping.Core.csproj)
shared data model and intensity value conversion to byte value (eg. 0.0~10.0 to 0 ~ 255)

Color Mapping Samples
#### Rainbow
![rainbow](image/Rainbow_Palette.bmp)
#### Ironbow
![ironbow](image/Ironbow_Palette.bmp)

### Wpf Canvas Drawing (WpfCanvasDrawing.csproj)
ROI graphics drawing overlays on the top of image view. Canvas drawing code were reference from https://github.com/songzhu/DrawToolsWPF and made necessary changes for ROI rectangle drawing.

### Wpf Intensity View (WpfIntensityView.csproj)
Data are mapped to RGB value and generate bitmap source using selected color mapping (eg. rainbow, ironbow).
Image view WpfUI control displaying data with selected color mapping

### Intensity Value Generation (IntensityValueGeneration.csproj)
utility classes which provides a periodic intensity generation for testing
(it can be considered as sample data source, instead of real data providers, sensor/thermal camera)

### Demo App net (DemoAppNet)
project demonstrates 2D intensity values (thermal data) to RGB image data display for WPF-UI application, using IntensityView library
![screenshot](image/demo.gif)

## Structure
```sh
Solution Items/
  .editorconfig
  Directory.Build.props
  README.md
src/
  IntensityMapping.Core/
  IntensityValueGeneration/
  WpfCanvasDrawing/
  WpfIntensityView/
sample/
  DemoAppNet/
  image/
test/
  IntensityMapping.Core.Tests/
  ```