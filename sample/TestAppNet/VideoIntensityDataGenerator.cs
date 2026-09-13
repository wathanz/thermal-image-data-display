using IntensityMapping.Core.Data;
using IntensityMapping.Core.Interface;
using OpenCvSharp;
using System;

namespace DemoAppNet;

internal sealed class VideoIntensityDataGenerator : IIntensityValueGenerator, IDisposable {
    private readonly VideoCapture capture;
    private readonly Mat frame = new();
    private readonly Mat grayscaleFrame = new();
    private readonly int outputWidth;
    private readonly int outputHeight;

    public VideoIntensityDataGenerator(string source, int outputWidth, int outputHeight) {
        this.outputWidth = outputWidth;
        this.outputHeight = outputHeight;

        capture = int.TryParse(source, out var cameraIndex)
            ? new VideoCapture(cameraIndex)
            : new VideoCapture(source);

        if (!capture.IsOpened()) {
            capture.Dispose();
            throw new InvalidOperationException($"Unable to open video source '{source}'.");
        }
    }

    public IntensityData<double> Generate() {
        if (!capture.Read(frame) || frame.Empty()) {
            capture.Set(VideoCaptureProperties.PosFrames, 0);
            if (!capture.Read(frame) || frame.Empty()) {
                throw new InvalidOperationException("The video source returned no frame.");
            }
        }

        Cv2.CvtColor(frame, grayscaleFrame, ColorConversionCodes.BGR2GRAY);
        using var resized = new Mat();
        Cv2.Resize(grayscaleFrame, resized, new OpenCvSharp.Size(outputWidth, outputHeight));

        var values = new double[outputWidth * outputHeight];
        for (var row = 0; row < outputHeight; row++) {
            for (var column = 0; column < outputWidth; column++) {
                values[row * outputWidth + column] = resized.At<byte>(row, column);
            }
        }

        return new IntensityData<double>(values, outputWidth, outputHeight, 0, 255);
    }

    public void Dispose() {
        grayscaleFrame.Dispose();
        frame.Dispose();
        capture.Dispose();
    }
}