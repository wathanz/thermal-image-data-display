
using DemoAppNet.Views;
using IntensityMapping.Core;
using IntensityMapping.Core.Data;
using IntensityMapping.Core.DataConverter;
using IntensityMapping.Core.Interface;
using IntensityValueGeneration;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfIntensityView.Interface;

namespace DemoAppNet;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    private IIntensityDataSource dataSource;
    private IntensityData<double> lastDisplayMap;
    private IIntensityDataMapBitmap intensityDataBitmapView;
    private IColorMapping colorMapping;
    private IValueGenerationConfig valueGenerationConfig;
    public MainWindow(IColorMapping colorMapping, IIntensityDataMapBitmap intensityBitmapView, IValueGenerationConfig valueGenerationConfig) {
        InitializeComponent();
        this.valueGenerationConfig = valueGenerationConfig;
        this.colorMapping = colorMapping;
        this.intensityDataBitmapView = intensityBitmapView;
    }

    private async void PaletteSelectionChanged(object sender, SelectionChangedEventArgs e) {
        await UpdateImageDisplayAsync(lastDisplayMap);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        var imageWidth = 1920;
        var imageHeight = 1080;
        valueGenerationConfig.Interval = 50;

        var patternGenerator = new GradientPatternGenerator(imageWidth, imageHeight, valueGenerationConfig.DataMin, valueGenerationConfig.DataMax);
        dataSource = new PeriodicIntensityDataSource(patternGenerator, valueGenerationConfig);
        GridSideView.DataContext = dataSource;
        GrdViewInfo.DataContext = ImgViewUI.Info;

        ColorMappingsSelection.ItemsSource = colorMapping.GetMappingKeys();
        ColorMappingsSelection.SelectedIndex = 0;


        dataSource.OnIntensityValuesChanged += MapGenerator_OnIntensityValuesChanged;
        dataSource.Start();

    }


    private async void MapGenerator_OnIntensityValuesChanged(object sender, MapChangedEventArgs args) {
        await UpdateImageDisplayAsync(args.Data, args.TrackingInfo);
        lastDisplayMap = args.Data;

    }

    private async Task UpdateImageDisplayAsync(IntensityData<double> mappedData, FpsTrackingInfo trackingInfo = null) {
        if (mappedData == null) {
            return;
        }

        var action = new Action(() => {
            if (ColorMappingsSelection.SelectedItem == null) return;
            var mappingName = ColorMappingsSelection.SelectedItem.ToString();
            var dataMin = valueGenerationConfig.DataMin;
            var dataMax = valueGenerationConfig.DataMax;

            // convert intensity value to 0~255 byte array
            var data = IntensityDataConverter.Convert(mappedData.Values, dataMin, dataMax, 0, 255);

            // update image display source data with selected color mapping
            intensityDataBitmapView.Update(mappedData.Width, mappedData.Height, data, mappingName, newImgeSize: out bool newImgeSize);
            ImgViewUI.UpdateImageSource(intensityDataBitmapView.WriteableSource, newImgeSize, trackingInfo);
        });


        if (Dispatcher.CheckAccess()) {
            action();
        }
        else {
            await Dispatcher.InvokeAsync(action);
        }
    }


    private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        var exitApplication = MessageBox.Show("Exit Application?", "Exit?", MessageBoxButton.YesNo)
            == MessageBoxResult.Yes;

        if (!exitApplication) {
            e.Cancel = true;
            return;
        }

        if (dataSource == null) return;
        await dataSource.StopAsync();
        dataSource.OnIntensityValuesChanged -= MapGenerator_OnIntensityValuesChanged;

    }

    private void BtnStart_Click(object sender, RoutedEventArgs e) {
        try {
            dataSource.Start();
        }
        catch (Exception ex) {
            ExceptionHandler.ShowMessage(ex, this);
        }
    }

    private async void BtnStop_Click(object sender, RoutedEventArgs e) {
        try {
            await dataSource.StopAsync();
        }
        catch (Exception ex) {
            ExceptionHandler.ShowMessage(ex, this);
        }
    }

    private void BtnResetView_Click(object sender, RoutedEventArgs e) {
        ImgViewUI.FitView();
    }

    private void BtnRoiInfo_Click(object sender, RoutedEventArgs e) {

        if (HasWindowsShow(typeof(RoiInfoWindow)))
            return;

        var win = new RoiInfoWindow(ImgViewUI);
        win.Owner = this;
        win.Show();
        win.Closed += ChildWindow_Closed;
    }

    private void ChildWindow_Closed(object sender, EventArgs e) {
        try {

            ((Window)sender).Closed -= ChildWindow_Closed;
            if (OwnedWindows.Count > 0) {
                OwnedWindows[0].Focus();
                return;
            }
            Focus();
        }
        catch (Exception ex) {
            ExceptionHandler.ShowMessage(ex, this);
        }
    }

    private bool HasWindowsShow(Type win) {
        foreach (var item in this.OwnedWindows) {
            if (item.GetType() == win) {
                (item as Window).Activate();
                return true;
            }
        }
        return false;
    }

    private void BtnTrackerInfo_Click(object sender, RoutedEventArgs e) {
        if (HasWindowsShow(typeof(DebuggingWindow)))
            return;

        var win = new DebuggingWindow(dataSource);
        win.Owner = this;
        win.Show();
        win.Closed += ChildWindow_Closed;
    }
}