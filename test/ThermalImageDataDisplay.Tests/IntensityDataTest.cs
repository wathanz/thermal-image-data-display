using IntensityMapping.Core.Data;

namespace ThermalImageDataDisplay.Tests;

public class IntensityDataTest {

    [Fact]
    public void Create_New_Created() {
        var width = 600; var height = 400;
        var data = new int[width * height];
        var intensityData = new IntensityData<int>(data, width, height, 0, 100);
        Assert.Equal(width, intensityData.Width);
        Assert.Equal(height, intensityData.Height);
        Assert.Equal(height * width, intensityData.Values.Length);
        Assert.Equal(0, intensityData.MinReference);
        Assert.Equal(100, intensityData.MaxReference);
    }

    [Theory]
    [InlineData(-1, 600, 400, 0, 100, "value", typeof(ArgumentNullException))]
    [InlineData(600 * 400 - 1, 600, 400, 0, 100, "value", typeof(ArgumentException))]
    [InlineData(100 * 400, 100, 400, 0, 100, "width", typeof(ArgumentException))]
    [InlineData(4000 * 400, 4000, 400, 0, 100, "width", typeof(ArgumentException))]
    [InlineData(600 * 100, 600, 100, 0, 100, "height", typeof(ArgumentException))]
    [InlineData(600 * 400, 600, 4000, 0, 100, "height", typeof(ArgumentException))]
    [InlineData(600 * 400, 600, 400, 99, 99, "maxReference", typeof(ArgumentException))]

    public void Create_New_ThrowException(int dataLength, int width, int height,
        double refDataMin, double refDataMax, string expectedStringOnMessage, Type type) {
        int[]? data = dataLength < 0 ? null :
                      new int[dataLength];
        var exception = Assert.ThrowsAny<Exception>(() => new IntensityData<int>(data, width, height, refDataMin, refDataMax));
        Assert.Equal(exception.GetType(), type);
        Assert.Contains(expectedStringOnMessage, exception.Message);
    }
}