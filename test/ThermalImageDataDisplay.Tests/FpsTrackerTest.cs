using IntensityMapping.Core;
using IntensityMapping.Core.Interface;
using Xunit.Abstractions;

namespace ThermalImageDataDisplay.Tests;
public class FpsTrackerTest {
    private readonly ITestOutputHelper output;

    public FpsTrackerTest(ITestOutputHelper output) {
        this.output = output;
    }

    [Theory]
    [InlineData(2000, 2001, 0.1, 0.6)]
    [InlineData(1000, 1001, 0.3, 1.5)]
    [InlineData(500, 501, 1.0, 3.0)]
    [InlineData(300, 350, 2.0, 4.0)]
    [InlineData(50, 60, 17, 22)]
    [InlineData(50, 55, 18, 21)]
    [InlineData(50, 50, 18.5, 21)]
    [InlineData(50, 55, 17.5, 21, 0.5)]
    [InlineData(50, 50, 18.5, 21, 3)]
    [InlineData(20, 22, 45, 52)]
    [InlineData(20, 22, 45, 52, 2)]
    public void TrackAndVerifyCalculatedFps(int sleepDurationFrom, int sleepDurationTo,
        double minExpectedFps, double maxExpectedFps, float totalLengthFactor = 1.0f) {
        var currentTime = DateTime.UnixEpoch;
        var tracker = new FpsTracker(() => currentTime) as IFpsTracker;
        tracker.Start();
        var totalDataLength = tracker.Records.Length;
        var diff = sleepDurationTo - sleepDurationFrom;
        var interval = sleepDurationFrom + diff / 2;
        var timeSpan = TimeSpan.FromSeconds(0);
        long count;
        var frameCounter = 0L;
        var lastCheckSecond = 0.0;

        //wait all data point are collected
        while (tracker.FpsRecordCounter < totalDataLength * totalLengthFactor) {
            currentTime = currentTime.AddMilliseconds(interval);
            tracker.Update(frameCounter++);

            var hasDefaultValue = tracker.Records.Contains(-1);
            count = tracker.FpsRecordCounter;
            if (count < totalDataLength) {
                Assert.True(hasDefaultValue, "should have default value");
            }
            else {
                Assert.False(hasDefaultValue, "should not have default value");
            }
            timeSpan += TimeSpan.FromMilliseconds(interval);

            if (count > 2 && tracker.FpsCalculated && (timeSpan.TotalSeconds - lastCheckSecond) > 1.0d) {
                lastCheckSecond = timeSpan.TotalSeconds;
                AssertFpsInRange(tracker.Fps, minExpectedFps, maxExpectedFps, "each second passed"); ;
            }
        }

        output.WriteLine($"PFS: {tracker.Fps}, expected range {minExpectedFps} ~ {maxExpectedFps}");
        output.WriteLine($"total-frame: {tracker.FrameCounter} times-span:{timeSpan.TotalSeconds:F2}");
        AssertFpsInRange(tracker.Fps, minExpectedFps, maxExpectedFps, "at the end");
    }

    private void AssertFpsInRange(float fps, double min, double max, string testFor) {
        var atTheEndInRange = fps >= min && fps <= max;
        Assert.True(atTheEndInRange, $"during {testFor} test, {fps} is out of range {min} ~ {max}");
    }

}