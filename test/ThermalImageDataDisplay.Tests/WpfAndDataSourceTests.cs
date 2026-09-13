using IntensityMapping.Core.Data;
using IntensityMapping.Core.Interface;
using IntensityValueGeneration;
using System.Windows;
using System.Windows.Media;
using WpfCanvasDrawing;
using WpfIntensityView.Model;

namespace ThermalImageDataDisplay.Tests;

public class WpfAndDataSourceTests {
    [Fact]
    public void ColorMapping_LoadsPalettesFromApplicationOutput() {
        var mapping = new ColorMapping();

        Assert.Contains("Rainbow", mapping.GetMappingKeys());
        Assert.Equal(256, mapping.GetBitmapPalette("Rainbow").Colors.Count);
    }

    [Fact]
    public void RoiRectangle_MovesAndTestsIntersection() {
        var roi = new GraphicsRoiRectangle(1, 10, 20, 30, 40, 1, Colors.Red, 1);

        roi.Move(5, -10);

        Assert.Equal(new Rect(15, 10, 30, 40), roi.Rectangle);
        Assert.True(roi.IntersectsWith(new Rect(20, 20, 5, 5)));
        Assert.Equal(0, roi.MakeHitTest(new Point(20, 20)));
    }

    [Theory]
    [InlineData(800, 600, 1600, 900, 0.5)]
    [InlineData(600, 800, 1600, 900, 0.375)]
    public void ZoomUtil_CalculatesFitZoom(double controlWidth, double controlHeight,
        double imageWidth, double imageHeight, double expectedZoom) {
        var zoom = ZoomUtil.CalculateFitZoom(controlWidth, controlHeight, imageWidth, imageHeight);

        Assert.Equal(expectedZoom, zoom, 6);
    }

    [Fact]
    public async Task PeriodicDataSource_StartsPublishesAndStops() {
        var config = new ValueGenerationConfig { Interval = 10 };
        using var source = new PeriodicIntensityDataSource(new TestGenerator(), config);
        using var signal = new ManualResetEventSlim();
        var eventCount = 0;
        source.OnIntensityValuesChanged += (_, _) => {
            Interlocked.Increment(ref eventCount);
            signal.Set();
        };

        source.Start();

        Assert.True(signal.Wait(TimeSpan.FromSeconds(2)));
        await source.StopAsync();

        Assert.True(eventCount > 0);
        Assert.True(source.Counter > 0);
        Assert.False(source.Tracker.Tracking);
    }

    private sealed class TestGenerator : IIntensityValueGenerator {
        public IntensityData<double> Generate() {
            return new IntensityData<double>(new double[240 * 240], 240, 240, 0, 1);
        }
    }
}