using IntensityMapping.Core;
using IntensityMapping.Core.Interface;
using System;
using System.Text;
using System.Windows;

namespace DemoAppNet.Views;
/// <summary>
/// Interaction logic for DebuggingWindow.xaml
/// </summary>
public partial class DebuggingWindow : Window {
    private readonly IIntensityDataSource intensityDataSource;

    public DebuggingWindow(IIntensityDataSource intensityDataSource) {
        InitializeComponent();
        this.intensityDataSource = intensityDataSource;
        this.intensityDataSource.OnIntensityValuesChanged += IntensityDataSource_OnIntensityValuesChanged;
    }

    private void IntensityDataSource_OnIntensityValuesChanged(object sender, MapChangedEventArgs args) {
        UpdateTextOutput();
    }

    private void UpdateTextOutput() {
        var action = () => {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"FPS Tracker Data {DateTime.Now.ToString("HH:mm:ss.fff")}");
            stringBuilder.AppendLine(string.Join(Environment.NewLine, intensityDataSource.Tracker.Records));
            TextOutput.Text = stringBuilder.ToString();
        };
        if (CheckAccess()) {
            action();
            return;
        }
        Dispatcher.BeginInvoke(action);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        intensityDataSource.OnIntensityValuesChanged -= IntensityDataSource_OnIntensityValuesChanged;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        if (!intensityDataSource.Tracker.Tracking) {
            UpdateTextOutput();
        }
    }
}