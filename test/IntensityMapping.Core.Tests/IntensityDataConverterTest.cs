using IntensityMapping.Core.DataConverter;

namespace IntensityMapping.Core.Tests;
public class IntensityDataConverterTest {

    [Fact]
    public void Convert_ValueRange_ShouldConverted() {
        var data = new double[100];
        for (int i = 0; i < data.Length; i++) {
            data[i] = i;
        }
        var convertedData = IntensityDataConverter.Convert(data, 0, 100, 0, 10);
        Assert.NotNull(convertedData);
        Assert.Equal(data.Length, convertedData.Length);
        for (byte i = 0; i < 10; i++) {
            var index = Array.IndexOf(convertedData, i);
            Assert.NotEqual(-1, index);
        }
    }


    [Theory]
    [InlineData(0, 100, 100, 200)]
    [InlineData(-1, 100, 100, 200)]
    [InlineData(10, 200, 100, 200)]
    [InlineData(11, 200, 100, 200)]
    [InlineData(5, 150, 100, 200)]
    public void Convert_Value_ShouldConverted(double value, byte expectedConvertedValue, byte mappingMin, byte mappingMax) {
        var data = new double[1];
        data[0] = value;
        const double dataRangeMin = 0;
        const double dataRangeMax = 10;
        var convertedData = IntensityDataConverter.Convert(data, dataRangeMin, dataRangeMax, mappingMin, mappingMax);
        Assert.NotNull(convertedData);
        Assert.Equal(expectedConvertedValue, convertedData[0]);
    }
}