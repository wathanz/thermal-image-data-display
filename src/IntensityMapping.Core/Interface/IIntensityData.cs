namespace IntensityMapping.Core.Interface;
public interface IIntensityData<T> {
    int Height { get; }
    T[] Values { get; }
    int Width { get; }
}