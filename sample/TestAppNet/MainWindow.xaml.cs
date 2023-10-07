
using IntensityView;
using IntensityView.Model.Data;
using IntensityView.Model.ValueSource;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DemoAppNet {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private PeriodicIntensityGenerator mapGenerator;
        private IntensityData<double> lastDisplayMap;
        private double minDataValue = 0.0f;
        private double maxDataValue = 10.0;
        private IntensityBitmapView imageView;

        public MainWindow() {
            InitializeComponent();
        }

        private void PaletteSelectionChanged(object sender, SelectionChangedEventArgs e) {
            UpdateImageDisplay(this.lastDisplayMap);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            imageView = new IntensityBitmapView();
            mapGenerator = new PeriodicIntensityGenerator();
            GridSideView.DataContext = mapGenerator;
            GrdViewInfo.DataContext = ImgViewUI.Info;

            mapGenerator.OnIntensityValuesChanged += MapGenerator_OnIntensityValuesChanged;
            mapGenerator.Start(400, 200, minDataValue, maxDataValue, TimeSpan.FromSeconds(0.1));

            ColorMappingsSelection.ItemsSource = ColorMapping.GetMappingKeys();
            ColorMappingsSelection.SelectedIndex = 0;

        }

        private void MapGenerator_OnIntensityValuesChanged(object sender, MapChangedEventArgs args) {
            UpdateImageDisplay(args.Data);
            this.lastDisplayMap = args.Data;

        }

        private void UpdateImageDisplay(IntensityData<double> mappedData) {
            if (mappedData == null) {
                return;
            }

            var action = new Action(() => {
                var mappingName = ColorMappingsSelection.SelectedItem.ToString();
                // convert intensity value to 0~255 byte array
                var data = IntensityDataConverter.Convert(mappedData.Values, minDataValue, this.maxDataValue, 0, 255);
                // update image display source data with selected color mapping
                imageView.Update(mappedData.Width, mappedData.Height, data, mappingName, out var newImgeSize);
                ImgViewUI.UpdateImageSource(imageView.WriteableSource, newImgeSize);



            });

            if (Dispatcher.CheckAccess()) {
                action.Invoke();
            }
            else {
                Dispatcher.Invoke(action);
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

            mapGenerator.OnIntensityValuesChanged -= MapGenerator_OnIntensityValuesChanged;
            if (mapGenerator != null) {
                mapGenerator.Stop();
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e) {
            mapGenerator.Start();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e) {
            mapGenerator.Stop();
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
    }
}