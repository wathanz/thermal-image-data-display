using IntensityMapping.Core.Interface;
using System.Diagnostics;
using Xunit.Abstractions;

namespace IntensityMapping.Core.Tests;
public class FpsTrackerTest : IDisposable {
    private CancellationTokenSource tokenSource = new();
    private readonly ITestOutputHelper output;

    public FpsTrackerTest(ITestOutputHelper output) {
        this.output = output;
    }
    public void Dispose() {
        tokenSource.Cancel();
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
    public async void TrackAndVerifyCalulatedFps(int sleepDurationFrom, int sleepDurationTo,
        double minExpectedFps, double maxExpectedFps, float totalLenghtFactor = 1.0f) {
        var tracker = new FpsTracker() as IFpsTracker;
        tracker.Start();
        var totalDataLenght = tracker.Records.Length;
        var startedTime = DateTime.Now;
        var totalTime = 0;
        var diff = sleepDurationTo - sleepDurationFrom;
        var random = new Random();
        var startTime = DateTime.Now;
        var timeSpan = TimeSpan.FromSeconds(0);
        long count;
        var frameCounter = 0L;
        var lastCheckSecond = 0.0;

        //wait all data point are collected
        while (tracker.FpsRecordCounter < totalDataLenght * totalLenghtFactor) {
            var internval = sleepDurationFrom + random.Next(diff);
            totalTime += internval;
            await WaitForInterval(totalTime, startedTime, tokenSource.Token);
            tracker.Update(frameCounter++);

            var hasDefaultValue = tracker.Records.Contains(-1);
            count = tracker.FpsRecordCounter;
            if (count < totalDataLenght) {
                Assert.True(hasDefaultValue, "should have default value");
            }
            else {
                Assert.False(hasDefaultValue, "should not have default value");
            }
            timeSpan = DateTime.Now - startTime;

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
        var atTheEndInrange = fps >= min && min <= max;
        Assert.True(atTheEndInrange, $"during {testFor} test, {min} is out of range {min} ~ {max}");
    }

    private async Task WaitForInterval(int duration, DateTime startedTime, CancellationToken token) {
        await Task.Run(async () => {
            while (true) {

                if (token.IsCancellationRequested)
                    break;

                if (DateTime.Now - startedTime >= TimeSpan.FromMilliseconds(duration)) {
                    return;
                }
                await Task.Delay(0);
            }
        }, token);


    }
}