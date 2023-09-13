using IntensityView.Model.Data;

namespace IntensityView.Model.ValueSource {

    internal class IntensityValueGenerator {
        private static int lastStartedCol = 0;

        // generate 2d intensity value with gradient pattern
        public static IntensityData<double> GenerateGradientPattern(int width, int height, double dataMin = 0, double dataMax = 10) {
            var data = new double[height * width];

            for (var j = 0; j < height; j++) {
                for (var i = 0; i < width; i++) {
                    var val = GetDataValueAtColumn(dataMin, dataMax, i, width, lastStartedCol);
                    data[j * width + i] = val;
                }
            }

            lastStartedCol += 1;
            if (lastStartedCol >= width) {
                lastStartedCol = 0;
            }

            return new IntensityData<double>(data, width, height);
        }

        private static double GetDataValueAtColumn(double min, double max, int columnIndex, int column, int lastStartedCol) {
            var diff = columnIndex - lastStartedCol;
            if (diff < 0) diff = column + diff;
            return min + (max - min) * (diff / (double)column);
        }


    }
}