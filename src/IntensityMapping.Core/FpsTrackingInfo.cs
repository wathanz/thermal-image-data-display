namespace IntensityMapping.Core;

public class FpsTrackingInfo {
    public float Fps { get; }
    public bool IsStarted { get; }
    public FpsTrackingInfo(float fps, bool started) {
        Fps = fps;
        IsStarted = started;
    }
}