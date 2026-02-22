using IntensityMapping.Core;
using IntensityMapping.Core.Data;
using IntensityMapping.Core.Interface;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace IntensityValueGeneration;


public class ValueGenerationConfig : IValueGenerationConfig {
    public double DataMin { get; set; } = 0;
    public double DataMax { get; set; } = 10;
    public int Interval { get; set; } = 1000;
}



/// <summary>
/// Utility class to Generate Intensity Values Periodically
/// </summary>
public class PeriodicIntensityDataSource : NotifyPropertyChangedBase, IIntensityDataSource, IDisposable {
    private bool isStarted = false;
    private bool disposed = false;
    private IIntensityValueGenerator valueGenerator = null;

    //callback for the Generated value changed
    public event IntensityValuesChangedHandler OnIntensityValuesChanged;
    private Timer timer;
    private ManualResetEventSlim evntOnElaspedExecuted = new ManualResetEventSlim(true);

    private IValueGenerationConfig config;
    private IFpsTracker tracker;
    private long counter = 0;
    public long Counter {
        get { return counter; }
        set { SetProperty(ref counter, value); }
    }

    public IFpsTracker Tracker { get { return tracker; } }


    public PeriodicIntensityDataSource(IIntensityValueGenerator intensityValueGenerator,
        IValueGenerationConfig config) {
        valueGenerator = intensityValueGenerator;
        tracker = new FpsTracker();
        timer = new Timer();
        timer.Interval = 1000;

        this.config = config;
    }


    private void TimerElapsed(object sender, ElapsedEventArgs e) {
        if (!evntOnElaspedExecuted.Wait(0)) return;
        try {
            evntOnElaspedExecuted.Reset();
            Generate();
        }
        catch (Exception ex) {
            StopTimmer();
            Console.WriteLine(ex.ToString());
        }
        finally {
            if (!timer.Enabled) {
                isStarted = false;
            }
            evntOnElaspedExecuted.Set();
        }
    }

    public void Start() {
        if (isStarted) {
            return;
        }

        timer.Interval = config.Interval;
        isStarted = true;
        tracker.Start();
        timer.Elapsed += TimerElapsed;
        timer.Start();
    }

    private void Generate() {

        Counter++;
        if (Counter == long.MaxValue) {
            Counter = 1;
        }

        tracker.Update(counter);
        var data = valueGenerator.Generate();
        if (OnIntensityValuesChanged != null) {
            OnIntensityValuesChanged(this, new MapChangedEventArgs(data, tracker.Fps, isStarted));
        }
    }
    private void StopTimmer() {
        timer.Elapsed -= TimerElapsed;
        timer.Stop();
    }
    public async Task StopAsync() {
        await Task.Run(() => {
            if (!isStarted) return;
            StopTimmer();
            if (!evntOnElaspedExecuted.Wait(2000)) {
                throw new InvalidOperationException("Stop Event wait time out");
            }
            tracker.Stop();
            isStarted = false;
        }).ConfigureAwait(false);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposed) return;
        if (disposing) {
            if (isStarted) {
                StopTimmer();
                evntOnElaspedExecuted.Wait(2000);
                tracker.Stop();
                isStarted = false;
            }
            timer.Dispose();
            evntOnElaspedExecuted.Dispose();
        }
        disposed = true;
    }
}