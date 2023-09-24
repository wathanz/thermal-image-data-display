using DrawingLib;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DemoAppNet {
    public partial class RoiInfoControl : UserControl {
        private readonly RoiRectangle roiRectangle;
        public event EventHandler OnRoiRemovedClicked;

        public RoiRectangle RoiRectangle { get { return this.roiRectangle; } }
        public RoiInfoControl(RoiRectangle roiRectangle, Size imageSize) {
            InitializeComponent();
            this.roiRectangle = roiRectangle;
            this.ImageSize = imageSize;
            this.DataContext = roiRectangle;



        }

        public Size ImageSize { get; }

        private void TxtColorEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            this.roiRectangle.TrySetColorString(TxtColorEntry.Text);
        }

        private void TxtLeftEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            var text = (sender as TextBox).Text;
            if (!int.TryParse(text, out var result)) return;
            if (!CheckIfInbetween(result, 0, (int)ImageSize.Width)) return;

            var newRight = result + this.roiRectangle.Width;
            if (!CheckIfInbetween(newRight, 0, (int)ImageSize.Width)) return;
            this.roiRectangle.Left = result;
        }

        private void TxtTopEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            var text = (sender as TextBox).Text;
            if (!int.TryParse(text, out var result)) return;
            if (!CheckIfInbetween(result, 0, (int)ImageSize.Height)) return;

            var newBottom = result + this.roiRectangle.Height;
            if (!CheckIfInbetween(newBottom, 0, (int)ImageSize.Height)) return;
            this.roiRectangle.Top = result;
        }



        private void TxtWidthEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            var text = (sender as TextBox).Text;
            if (!int.TryParse(text, out var result)) return;
            if (!CheckIfInbetween(result, 0, (int)ImageSize.Width)) return;

            var newRight = result + this.roiRectangle.Left;
            if (!CheckIfInbetween(newRight, 0, (int)ImageSize.Width)) return;
            this.roiRectangle.Width = result;
        }



        private void TxtHeightEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            var text = (sender as TextBox).Text;
            if (!int.TryParse(text, out var result)) return;
            if (!CheckIfInbetween(result, 0, (int)ImageSize.Height)) return;

            var newBottom = result + this.roiRectangle.Top;
            if (!CheckIfInbetween(newBottom, 0, (int)ImageSize.Height)) return;
            this.roiRectangle.Height = result;
        }

        private void TxtThicknessEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            var text = (sender as TextBox).Text;
            if (!double.TryParse(text, out var result)) return;
            if (result <= 0 && result > 20) return;
            this.roiRectangle.Thickness = result;
        }

        private static bool CheckIfInbetween(int value, int min, int max) {
            return value > min && value < max;
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e) {
            if (OnRoiRemovedClicked != null)
                OnRoiRemovedClicked(this, e);
        }


        private void TxtTextBottom_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            this.roiRectangle.TextOnBottom = TxtTextBottom.Text;
        }

        private void TxtTextTop_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            this.roiRectangle.TextOnTop = TxtTextTop.Text;
        }
    }
}