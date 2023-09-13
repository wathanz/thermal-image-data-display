## intensity wiew (IntensityView.csproj)
data model and intensity value conversion to byte value (eg. 0.0~10.0 to 0 ~ 255)
data map to RGB value and generate bitmap source using color mapping (eg. rainbow, ironbow) 
## Rainbow
![rainbow](image/Rainbow_Palette.bmp)

## Ironbow
![ironbow](image/Ironbow_Palette.bmp)

## intensity value generation (IntensityValueGeneration.csproj)
utility classes providing a periodic intensity generator for testing
(it can be considered was data source, instead of real sensor/thermal camera)

# demo app net (DemoAppNet)
project demostrates 2D intensity values (thermal data) to RGB image data display for WPF-UI applicaiton, using IntensityView library

![screenshot](image/demo.gif)