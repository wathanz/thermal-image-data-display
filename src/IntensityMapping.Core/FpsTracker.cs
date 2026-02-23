using IntensityMapping.Core.Interface;
using System;
using System.Linq;
namespace IntensityMapping.Core;
public class FpsTracker : IFpsTracker {

    DateTime lastCounterRecordedTime = DateTime.Now;
    long lastRecordedCounter = 0;
    private long fpsRecordCounter = 0;
    private const int averageDataLength = 10;
    private float[] records = new float[averageDataLength];
    private long frameCounter = 0;

    private bool tracking = false;
    private bool fpsCalculated = false;
    public bool Tracking => tracking;

    private float fps = 0;
    public float Fps {
        get { return fps; }
    }
    public long FrameCounter {
        get { return frameCounter; }
    }
    public long FpsRecordCounter {
        get { return fpsRecordCounter; }
    }

    public bool FpsCalculated {
        get { return fpsCalculated; }
    }
    public FpsTracker() {
        Reset();
    }

    public float[] Records {
        get {
            lock (records) {
                return records.ToArray();
            }
        }
    }

    private void Reset() {
        if (tracking) return;
        lock (records) {
            for (var i = 0; i < records.Length; i++) {
                records[i] = -1;
            }
        }
    }

    public void Start() {
        if (tracking) return;
        lastCounterRecordedTime = DateTime.Now;
        lastRecordedCounter = frameCounter;
        tracking = true;
    }
    public void Stop() {
        tracking = false;
    }
    public void Update(long frameCounter) {
        if (!tracking) return;

        this.frameCounter = frameCounter;
        if (fpsRecordCounter == long.MaxValue) {
            fpsRecordCounter = 1;
        }

        lock (records) {
            var now = DateTime.Now;
            var frameDifferent = (int)(frameCounter - lastRecordedCounter);
            var totalSecondPassed = (now - lastCounterRecordedTime).TotalSeconds;
            if (frameDifferent < 1 || totalSecondPassed < 1) {
                return;
            }

            var recordEntryIndex = fpsRecordCounter % averageDataLength;
            records[recordEntryIndex] = (float)((frameCounter - lastRecordedCounter) / totalSecondPassed);
            lastRecordedCounter = frameCounter;
            fpsRecordCounter++;
            lastCounterRecordedTime = now;

            fps = fpsRecordCounter > averageDataLength ?
                (float)records.Average() :
                (float)records.Where(x => x != -1).Average();

            fpsCalculated = true;
        }
    }
}