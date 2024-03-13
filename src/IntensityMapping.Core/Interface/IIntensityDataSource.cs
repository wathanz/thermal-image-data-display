using IntensityMapping.Core;
using IntensityMapping.Core.Interface;
using System.Threading.Tasks;

namespace IntensityValueGeneration;

public delegate void IntensityValuesChangedHandler(object sender, MapChangedEventArgs args);
public interface IIntensityDataSource {
    long Counter { get; }
    IFpsTracker Tracker { get; }

    event IntensityValuesChangedHandler OnIntensityValuesChanged;

    void Start();
    Task StopAsync();
}