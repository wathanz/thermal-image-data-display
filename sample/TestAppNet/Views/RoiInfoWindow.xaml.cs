
using DemoAppNet.Views;
using System;
using System.Windows;
using WpfIntensityView.Interface;

namespace DemoAppNet;
public partial class RoiInfoWindow : Window {
    private readonly IImageViewUI imageView;
    private Size imageSize;
    public RoiInfoWindow(IImageViewUI imageView) {
        InitializeComponent();
        this.imageView = imageView;
        imageSize = imageView.Info.ImageSize;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        UpdateRoiList();
    }

    private void UpdateRoiList() {
        ExceptionHandler.TryCatchDisplayException(() => {
            ListBoxRois.Items.Clear();
            var rois = imageView.GetRoiRectangles();
            foreach (var item in rois) {
                var control = new RoiInfoControl(item, imageSize);
                ListBoxRois.Items.Add(control);
                control.OnRoiRemovedClicked += Control_OnRoiRemovedClicked;
            }
        });
    }

    private void Control_OnRoiRemovedClicked(object sender, EventArgs e) {
        ExceptionHandler.TryCatchDisplayException(() => {
            var obj = sender as RoiInfoControl;
            if (obj == null) return;
            var roiRectangle = obj.RoiRectangle;
            if (MessageBox.Show(this, $"Remove {roiRectangle.RoiId} <{roiRectangle.TextOnTop}>?", "Remove ROI?",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            obj.OnRoiRemovedClicked -= Control_OnRoiRemovedClicked;
            imageView.RemoveRoi(roiRectangle);
            UpdateRoiList();
        });
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e) {
        ExceptionHandler.TryCatchDisplayException(() => {
            var l = (int)(imageView.Info.ImageWidth * 0.05f);
            var t = (int)(imageView.Info.ImageHeight * 0.05f);
            var w = (int)(imageView.Info.ImageWidth * 0.10f);
            var h = (int)(imageView.Info.ImageHeight * 0.10f);
            imageView.AddNewRoi(l, t, w, h);
            UpdateRoiList();
        });
    }
}