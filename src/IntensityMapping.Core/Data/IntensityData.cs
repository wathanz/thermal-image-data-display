using IntensityMapping.Core.Interface;
using System;

namespace IntensityMapping.Core.Data;
public class IntensityData<T> : IIntensityData<T> {
    public T[] Values { get; }
    public int Width { get; }
    public int Height { get; }

    public double MinReference { get; }
    public double MaxReference { get; }
    public IntensityData(T[] value, int width, int height, double minReference, double maxReference) {

        if (value == null) {
            throw new ArgumentNullException(nameof(value));
        }

        //4k = 3840 × 2160
        //validate if width and height are within the range
        if (width < 240 || width > 3840) {
            throw new ArgumentException($"{nameof(width)} must be more than 240 ~ 3840");
        }

        if (height < 240 || height > 2160) {
            throw new ArgumentException($"{nameof(height)} must be more than 240 ~ 2160");
        }

        var arrayLength = height * width;
        if (value.Length != arrayLength) {
            throw new ArgumentException($"{nameof(value)} length is expected {arrayLength}");
        }

        if (maxReference - minReference < 1) {
            throw new ArgumentException($"{nameof(maxReference)}-{nameof(minReference)} gap is less than 1");
        }

        Values = value;
        Width = width;
        Height = height;
        MinReference = minReference;
        MaxReference = maxReference;
    }
}