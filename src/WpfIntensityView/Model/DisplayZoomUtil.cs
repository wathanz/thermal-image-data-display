using System;

namespace WpfIntensityView.Model;
internal static class ZoomUtil {

    private readonly static double screenMarginOffset = 0;
    private readonly static double minScreenSize = 20;

    private static double CalculateFitScreenValue(double pixelW, double pixelH,
        double screenW, double screenH) {
        var z = 1.0;

        var screenRatio = screenW / screenH;
        var imgRatio = pixelW / pixelH;


        if (screenRatio > imgRatio)
            z = screenH / pixelH;
        else
            z = screenW / pixelW;
        return z;
    }

    public static double CalculateFitZoom(double actualControlWidth, double actualControlHeight,
        double newImageWidth, double newImageHeight) {

        var screenHeight = Math.Min(actualControlHeight, actualControlHeight - screenMarginOffset);
        var screenWidth = Math.Min(actualControlWidth, actualControlWidth - screenMarginOffset);

        if (screenHeight < minScreenSize || screenWidth < minScreenSize) {
            screenHeight = Math.Max(actualControlHeight, actualControlHeight - minScreenSize);
            screenWidth = Math.Max(actualControlWidth, actualControlWidth);
        }
        return CalculateFitScreenValue((int)newImageWidth, (int)newImageHeight, (int)screenWidth, (int)screenHeight);
    }
}