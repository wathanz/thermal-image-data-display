using IntensityView.Model;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace DrawingLib {
    public class RoiRectangle : NotifyPropertyChangedBase {

        private int left = 0;
        public int Left {
            get { return left; }
            set {
                if (!SetProperty(ref left, value)) return;
                this.rectangle.Left = value;
                this.UpdateRoiSize();
            }
        }
        private int top = 0;
        public int Top {
            get { return top; }
            set {
                if (!SetProperty(ref top, value)) return;
                this.rectangle.Top = value;
                this.UpdateRoiSize();
            }
        }
        private int width = 0;
        public int Width {
            get { return width; }
            set {
                if (!SetProperty(ref width, value)) return;
                this.UpdateRoiSize();
            }
        }

        private int height = 0;
        public int Height {
            get { return height; }
            set {
                if (!SetProperty(ref height, value)) return;
                this.UpdateRoiSize();
            }
        }

        private double thickness = 0;
        public double Thickness {
            get { return thickness; }
            set {
                if (SetProperty(ref thickness, value)) {
                    this.rectangle.LineWidth = value;
                }
            }
        }

        private string id = "";
        public string Id {
            get { return id; }
        }


        private bool isVisible = true;
        public bool IsVisible {
            get { return isVisible; }
            set {
                if (SetProperty(ref isVisible, value)) {
                    this.rectangle.IsHide = !value;
                }
            }
        }
        private string positionSizeInfo = "";
        public string PositionSizeInfo {
            get { return positionSizeInfo; }
            private set {
                SetProperty(ref positionSizeInfo, value);
            }
        }

        private string colorString = "";
        public string ColorString {
            get { return colorString; }
            private set {
                SetProperty(ref colorString, value);
            }
        }

        private void UpdateRoiSize() {
            this.rectangle.Right = this.left + this.width;
            this.rectangle.Bottom = this.top + this.height;
            this.rectangle.RefreshDrawing();
        }
        public bool TrySetColorString(string newValue) {
            try {
                var color = (Color)ColorConverter.ConvertFromString(newValue);
                this.rectangle.ObjectColor = color;
                return true;
            }
            catch (Exception) {
            }
            return false;
        }

        GraphicsRectangle rectangle = null;
        public GraphicsRectangle GraphicsRectangle { get { return rectangle; } }
        public RoiRectangle(GraphicsRectangle rectangle, int counter) : base() {

            this.rectangle = rectangle;
            this.id = $"ROI-{counter:00}";
            this.left = (int)rectangle.Left;
            this.top = (int)rectangle.Top;
            this.width = (int)Math.Abs(rectangle.Right - rectangle.Left);
            this.height = (int)Math.Abs(rectangle.Bottom - rectangle.Top);
            this.rectangle.DisplayText = this.id;
            this.thickness = rectangle.LineWidth;
            this.ColorString = rectangle.ObjectColor.ToString();
            PositionSizeInfo = $"{this.left:0}, {this.top:0}, {this.width:0}, {this.height:0}";

            rectangle.DrawingChanged += Rectangle_DrawingChanged;
        }

        private void Rectangle_DrawingChanged(object sender, EventArgs e) {
            this.UpdateInfo();
        }

        private void UpdateInfo() {

            var rec = rectangle.Rectangle;
            if (Threshold.IsEqual(this.left, (int)rec.Left, 1) &&
                Threshold.IsEqual(this.top, (int)rec.Top, 1) &&
                Threshold.IsEqual(this.width, (int)rec.Width, 1) &&
                Threshold.IsEqual(this.height, (int)rec.Height, 1))
                return;

            this.left = (int)rec.Left;
            this.top = (int)rec.Top;
            this.width = (int)rec.Width;
            this.height = (int)rec.Height;

            OnPropertyChanged(nameof(Left));
            OnPropertyChanged(nameof(Top));
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Height));

            PositionSizeInfo = $"{this.left:0}, {this.top:0}, {this.width:0}, {this.height:0}";
            ColorString = rectangle.ObjectColor.ToString();
        }

        ~RoiRectangle() {
            rectangle.DrawingChanged -= Rectangle_DrawingChanged;
        }
    }
}