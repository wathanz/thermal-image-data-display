using IntensityView.Interface;
using System;
using System.Windows;

namespace DemoAppNet {
    public partial class RoiInfoWindow : Window {
        private readonly IImageViewUI imageView;
        private Size imageSize;
        public RoiInfoWindow(IImageViewUI imageView) {
            InitializeComponent();
            this.imageView = imageView;
            this.imageSize = imageView.Info.ImageSize;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            UpdateRoiList();
        }

        private void UpdateRoiList() {
            try {
                ListBoxRois.Items.Clear();
                var rois = imageView.GetRoiRectangles();
                foreach (var item in rois) {
                    var control = new RoiInfoControl(item, this.imageSize);
                    ListBoxRois.Items.Add(control);
                    control.OnRoiRemovedClicked += Control_OnRoiRemovedClicked;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Control_OnRoiRemovedClicked(object sender, EventArgs e) {
            try {
                var obj = sender as RoiInfoControl;
                if (obj == null) return;

                obj.OnRoiRemovedClicked -= Control_OnRoiRemovedClicked;
                this.imageView.RemoveRoi(obj.RoiRectangle);
                UpdateRoiList();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e) {
            try {
                this.imageView.AddNewRoi();
                UpdateRoiList();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}