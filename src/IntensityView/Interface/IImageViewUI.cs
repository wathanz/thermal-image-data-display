using DrawingLib;
using IntensityView.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace IntensityView.Interface {
    public interface IImageViewUI {
        ImageViewInfo Info { get; }

        void AddNewRoi(int left = 5, int top = 5, int width = 50, int height = 30);
        void FitView(double width, double height);
        IEnumerable<RoiRectangle> GetRoiRectangles();
        void InitializeComponent();
        void RemoveRoi(RoiRectangle roi);
        void UpdateImageSource(BitmapSource bitmapSource, bool newImageSize);
    }
}