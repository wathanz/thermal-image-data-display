using IntensityView.Model;
using System;
using System.Windows;

namespace IntensityView.Views {
    public class ImageViewInfo : NotifyPropertyChangedBase {
        private int imageWidth = 0;
        public int ImageWidth {
            get { return imageWidth; }
            set {
                if (!SetProperty(ref imageWidth, value)) return;
                imageSize.Width = value;
            }
        }

        private int imageHeight = 0;
        public int ImageHeight {
            get { return imageHeight; }
            set {

                if (!SetProperty(ref imageHeight, value)) return;
                imageSize.Height = value;
            }
        }

        private double zoom = 0;
        public double Zoom {
            get { return zoom; }
            set {
                SetProperty(ref zoom, value);
            }
        }

        private Size imageSize = new Size();
        public Size ImageSize {
            get {
                return imageSize;
            }
        }
    }
}