using IntensityMapping.Core;
using System.Windows;

namespace WpfIntensityView.Views;
public class ImageViewInfo : NotifyPropertyChangedBase {
    private int imageWidth = 0;
    public int ImageWidth {
        get { return imageWidth; }
        set {
            if (!SetProperty(ref imageWidth, value)) return;
            imageSize.Width = value;
        }
    }

    private float fps = 0;
    public float Fps {
        get { return fps; }
        set {
            if (!SetProperty(ref fps, value)) return;
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

    private Size imageSize = new();
    public Size ImageSize {
        get {
            return imageSize;
        }
    }
}