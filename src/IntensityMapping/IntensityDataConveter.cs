namespace IntensityMapping {
    public class IntensityDataConverter {

        private static readonly byte DataMax = 255;
        private static readonly byte DataMin = 0;

        private static double[] CalculateFactor(double fromMin, double fromMax, double toMin = 0, double toMax = 255) {
            var m = (toMax - toMin) / (fromMax - fromMin);
            var c = toMin - fromMin * m;
            return new double[] { m, c };
        }

        public static byte[] Convert(double[] data, double fromMin, double fromMax, double toMin, double toMax) {
            var result = new byte[data.Length];
            var coefficient = CalculateFactor(fromMin, fromMax, toMin, toMax);
            for (int i = 0; i < data.Length; i++) {
                var mapValue = coefficient[0] * data[i] + coefficient[1];
                if (mapValue > DataMax)
                    result[i] = DataMax;
                else if (mapValue < DataMin)
                    result[i] = DataMin;
                else
                    result[i] = (byte)mapValue;
            }
            return result;
        }
    }
}