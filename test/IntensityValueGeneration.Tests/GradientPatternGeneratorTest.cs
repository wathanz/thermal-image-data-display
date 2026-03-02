using IntensityValueGeneration;

namespace IntensityValueGeneration.Tests;

public class GradientPatternGeneratorTest {

    [Fact]
    public void Generate_ReturnsCorrectDimensions() {
        var width = 640;
        var height = 480;
        var generator = new GradientPatternGenerator(width, height);
        var result = generator.Generate();
        Assert.Equal(width, result.Width);
        Assert.Equal(height, result.Height);
        Assert.Equal(width * height, result.Values.Length);
    }

    [Fact]
    public void Generate_ValuesWithinDataRange() {
        var width = 320;
        var height = 240;
        double dataMin = 2.0;
        double dataMax = 8.0;
        var generator = new GradientPatternGenerator(width, height, dataMin, dataMax);
        var result = generator.Generate();
        foreach (var value in result.Values) {
            Assert.True(value >= dataMin && value <= dataMax,
                $"Value {value} is out of range [{dataMin}, {dataMax}]");
        }
    }

    [Fact]
    public void Generate_MinAndMaxReferencePreserved() {
        double dataMin = -5.0;
        double dataMax = 15.0;
        var generator = new GradientPatternGenerator(640, 480, dataMin, dataMax);
        var result = generator.Generate();
        Assert.Equal(dataMin, result.MinReference);
        Assert.Equal(dataMax, result.MaxReference);
    }

    [Theory]
    [InlineData(320, 240)]
    [InlineData(640, 480)]
    [InlineData(1280, 720)]
    public void Generate_SuccessiveCallsShiftGradient(int width, int height) {
        var generator = new GradientPatternGenerator(width, height);
        var first = generator.Generate();
        var second = generator.Generate();
        Assert.False(first.Values.SequenceEqual(second.Values),
            "Successive Generate() calls should produce shifted gradient data");
    }
}
