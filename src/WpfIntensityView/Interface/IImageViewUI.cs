using IntensityMapping.Core;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using WpfCanvasDrawing;
using WpfIntensityView.Views;

namespace WpfIntensityView.Interface;
public interface IImageViewUI {
    ImageViewInfo Info { get; }
    void AddNewRoi(int left = 5, int top = 5, int width = 50, int height = 30);
    void FitView(double width, double height);
    IEnumerable<GraphicsRoiRectangle> GetRoiRectangles();
    void InitializeComponent();
    void RemoveRoi(GraphicsRoiRectangle roi);
    void UpdateImageSource(BitmapSource bitmapSource, bool newImageSize, FpsTrackingInfo trackingInfo);
}