using IntensityMapping.Core.Data;
using System;

namespace IntensityMapping.Core;

public class MapChangedEventArgs : EventArgs {
    public IntensityData<double> Data { get; }
    public FpsTrackingInfo TrackingInfo { get; }
    public MapChangedEventArgs(IntensityData<double> data, float fps, bool started) {
        Data = data;
        TrackingInfo = new FpsTrackingInfo(fps, started);
    }
}