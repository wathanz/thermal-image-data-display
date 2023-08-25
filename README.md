# intensity-image-data-display
project demostrates 2D intensity values (thermal data) to RGB image data display for WPF-UI applicaiton 

## intensity mapping class library (IntensityMapping.csproj)
data model and intensity value conversion to byte value (eg. 0.0~10.0 to 0 ~ 255)
also include a periodic intensity generator for intensity value genreation 
(it may come from any sensor as well. eg. thermal sensor)

## wpf test app (TestApp.csproj)
image display with color mapping test applicaion for 2D intensity map 

![screenshot](image/demo.gif)