using WpfCanvasDrawing;

namespace WpfCanvasDrawing.Tests;

public class MathHelperTest {

    [Theory]
    [InlineData(5, 1, 10, true)]
    [InlineData(1, 1, 10, true)]
    [InlineData(10, 1, 10, true)]
    [InlineData(0, 1, 10, false)]
    [InlineData(11, 1, 10, false)]
    [InlineData(-5, -10, -1, true)]
    [InlineData(-10, -10, -1, true)]
    [InlineData(-1, -10, -1, true)]
    [InlineData(-11, -10, -1, false)]
    [InlineData(0, -10, -1, false)]
    public void CheckIfInbetween_ReturnsExpected(int value, int min, int max, bool expected) {
        Assert.Equal(expected, MathHelper.CheckIfInbetween(value, min, max));
    }
}
