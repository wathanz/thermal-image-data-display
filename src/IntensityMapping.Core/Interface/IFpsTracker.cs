namespace IntensityMapping.Core.Interface;

public interface IFpsTracker {
    float Fps { get; }
    bool Tracking { get; }
    float[] Records { get; }
    bool FpsCalculated { get; }
    long FpsRecordCounter { get; }
    long FrameCounter { get; }
    void Start();
    void Stop();
    void Update(long frameCounter);
}