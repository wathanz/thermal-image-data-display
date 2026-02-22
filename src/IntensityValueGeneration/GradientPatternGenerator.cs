using IntensityMapping.Core.Data;
using IntensityMapping.Core.Interface;

namespace IntensityValueGeneration;
public class GradientPatternGenerator : IIntensityValueGenerator {
    private readonly int height;
    private readonly double dataMin;
    private readonly double dataMax;
    private readonly int width;
    private long lastStartedCol = 0;

    public GradientPatternGenerator(int width, int height, double dataMin = 0, double dataMax = 10) {
        this.width = width;
        this.height = height;
        this.dataMin = dataMin;
        this.dataMax = dataMax;
    }

    // generate 2d intensity value with gradient pattern
    public IntensityData<double> Generate() {
        var data = new double[height * width];
        lastStartedCol = lastStartedCol % width;
        for (var i = 0; i < width; i++) {
            var colVol = CalculateDataValueAtColumn(dataMin, dataMax, i, width, lastStartedCol); ;
            for (var j = 0; j < height; j++) {
                data[j * width + i] = colVol;
            }
        }
        lastStartedCol++;
        return new IntensityData<double>(data, width, height, dataMin, dataMax);
    }

    private double CalculateDataValueAtColumn(double min, double max, int columnIndex, int column, long lastStartedCol) {
        var diff = columnIndex - lastStartedCol;
        if (diff < 0) diff = column + diff;
        return min + (max - min) * (diff / (double)column);
    }


}
