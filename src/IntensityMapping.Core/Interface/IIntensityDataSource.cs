using IntensityMapping.Core;
using System.Threading.Tasks;

namespace IntensityMapping.Core.Interface;

public delegate void IntensityValuesChangedHandler(object sender, MapChangedEventArgs args);
public interface IIntensityDataSource {
    long Counter { get; }
    IFpsTracker Tracker { get; }

    event IntensityValuesChangedHandler OnIntensityValuesChanged;

    void Start();
    Task StopAsync();
}