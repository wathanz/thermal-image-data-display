using System.Collections;
using System.Collections.Generic;

namespace IntensityMapping.Core.DataConverter;
public class IntensityDataConverter {

    private static byte CapByte(double value, byte toMin, byte toMax) {
        if (value > toMax)
            return toMax;
        if (value < toMin)
            return toMin;
        return (byte)value;
    }
    private static double[] CalculateFactor(double fromMin, double fromMax, double toMin = 0, double toMax = 255) {
        var m = (toMax - toMin) / (fromMax - fromMin);
        var c = toMin - fromMin * m;
        return new double[] { m, c };
    }

    public static byte[] Convert(double[] data, double fromMin, double fromMax, byte toMin = 0, byte toMax = 255) {
        var result = new byte[data.Length];
        var coefficient = CalculateFactor(fromMin, fromMax, toMin, toMax);

        for (int i = 0; i < data.Length; i++) {
            var mapValue = coefficient[0] * data[i] + coefficient[1];
            result[i] = CapByte(mapValue, toMin, toMax);
        }

        return result;
    }
}