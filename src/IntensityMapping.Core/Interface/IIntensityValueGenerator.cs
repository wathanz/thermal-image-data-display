using IntensityMapping.Core.Data;

namespace IntensityMapping.Core.Interface;
public interface IIntensityValueGenerator {
    IntensityData<double> Generate();
}