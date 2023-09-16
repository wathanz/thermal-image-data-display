using System;

namespace IntensityView.Model.Data {
    public class IntensityData<T> {
        public T[] Values { get; }
        public int Width { get; }
        public int Height { get; }
        public IntensityData(T[] value, int width, int height) {
            Values = value;
            Width = width;
            Height = height;
        }
    }
}