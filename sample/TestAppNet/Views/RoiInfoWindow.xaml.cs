using DrawToolsLib;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DemoAppNet {
    public partial class RoiInfoWindow : Window {
        private readonly IEnumerable<GraphicsRectangle> graphicsRectangles;

        public RoiInfoWindow(IEnumerable<GraphicsRectangle> graphicsRectangles) {
            InitializeComponent();
            this.graphicsRectangles = graphicsRectangles;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            try {
                foreach (var item in this.graphicsRectangles) {
                    var control = new RoiInfoControl(item);
                    ListBoxRois.Items.Add(control);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}