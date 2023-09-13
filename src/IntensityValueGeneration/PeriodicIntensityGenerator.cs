using IntensityView.Model.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IntensityValueGeneration {

    public class MapChangedEventArgs : EventArgs {
        public IntensityData<double> Data { get; }
        public MapChangedEventArgs(IntensityData<double> data) {
            Data = data;
        }
    }

    public delegate void IntensityValuesChangedHandler(object sender, MapChangedEventArgs args);

    /// <summary>
    /// It is utility class to Generate Intensity Values Periodically
    /// </summary>
    public class PeriodicIntensityGenerator : NotifyPropertyChangedBase {
        private readonly ManualResetEvent eventStop = new ManualResetEvent(false);
        private bool isStarted = false;
        private int height = 5;
        private int width = 3;
        private TimeSpan interval = TimeSpan.FromSeconds(1);

        //callback for the Generated value changed
        public event IntensityValuesChangedHandler OnIntensityValuesChanged;

        private long counter = 0;
        public long Counter {
            get { return counter; }
            set { SetProperty(ref counter, value); }
        }

        private double dataMin = 0;
        public double DataMin {
            get { return dataMin; }
            set { SetProperty(ref dataMin, value); }
        }

        private double dataMax = 0;
        public double DataMax {
            get { return dataMax; }
            set { SetProperty(ref dataMax, value); }
        }

        public void Start() {
            if (isStarted) {
                return;
            }
            this.eventStop.Reset();
            this.isStarted = true;
            Task.Run(() => { Generate(); }).ContinueWith(e => CheckoutException(e));

        }

        public void Start(int width, int height, double dataMin, double dataMax, TimeSpan interval) {

            this.width = width;
            this.height = height;
            this.DataMin = dataMin;
            this.DataMax = dataMax;
            this.interval = interval;
            this.Start();

        }


        private void CheckoutException(Task e) {
            isStarted = false;
            if (e.Status == TaskStatus.Faulted) {
                Console.WriteLine(e.Exception.ToString());
            }
        }

        private void Generate() {
            var lastGeneratedTime = DateTime.Now;
            while (true) {

                if (eventStop.WaitOne(1)) break;
                if ((DateTime.Now - lastGeneratedTime) < interval) {
                    Thread.Sleep(2);
                    continue;
                }

                var data = IntensityValueGenerator.GenerateGradientPattern(this.width, this.height, this.dataMin, this.dataMax);
                lastGeneratedTime = DateTime.Now;
                Counter++;

                if (OnIntensityValuesChanged != null) {
                    OnIntensityValuesChanged(this, new MapChangedEventArgs(data));
                }
            }
        }

        public void Stop() {
            if (!isStarted) return;
            eventStop.Set();
            Thread.Sleep(10);
        }

        ~PeriodicIntensityGenerator() {
            Stop();
        }
    }
}