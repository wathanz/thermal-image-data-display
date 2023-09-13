using DrawingLib;
using IntensityView.Interface;
using IntensityView.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntensityView.Views {
    /// <summary>
    /// Interaction logic for ImageViewUI.xaml
    /// </summary>
    public partial class ImageViewUI : UserControl, IImageViewUI {
        public ImageViewUI() {
            InitializeComponent();
            this.Info = new ImageViewInfo();
        }

        private void GrdImageView_MouseDown(object sender, MouseButtonEventArgs e) {

        }

        private void GrdImageView_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {


            this.DataContext = Info;

            ImageHost.MouseLeftButtonDown += GrdDrawContainer_MouseLeftButtonDown;
            ImageHost.MouseLeftButtonUp += GrdDrawContainer_MouseLeftButtonUp;
            ImageHost.MouseMove += GrdDrawContainer_MouseMove;


        }
        public ImageViewInfo Info { get; }

        static int roiCount = 0;
        public IEnumerable<RoiRectangle> GetRoiRectangles() {
            var list = new List<RoiRectangle>();
            roiCount = 1;
            foreach (var item in OverlayCanvas.GetGraphics<GraphicsRectangle>()) {
                list.Add(new RoiRectangle(item, roiCount++));
            }
            return list;
        }
        public void AddNewRoi(int left = 5, int top = 5, int width = 50, int height = 30) {
            if (OverlayCanvas.Count > 20) throw new Exception($"Roi Max Limit");
            OverlayCanvas.GraphicsList.Add(new GraphicsRectangle(left, top, width, height, 1, Colors.Blue, OverlayCanvas.ActualScale));
            OverlayCanvas.RefreshClip();
        }
        public void RemoveRoi(RoiRectangle roi) {
            if (roi == null) return;
            HelperFunctions.Remove(OverlayCanvas, roi.GraphicsRectangle);
        }

        private void GrdDrawContainer_MouseMove(object sender, MouseEventArgs e) {
            if (!ImageHost.IsMouseCaptured) return;
            var p = e.MouseDevice.GetPosition(BorderImageView);
            var m = GrdDrawContainer.RenderTransform.Value;
            m.OffsetX = origin.X + (p.X - startMove.X);
            m.OffsetY = origin.Y + (p.Y - startMove.Y);
            GrdDrawContainer.RenderTransform = new MatrixTransform(m);
        }

        private void GrdDrawContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (!AllZoomPan) return;
            ImageHost.CaptureMouse();
            startMove = e.GetPosition(BorderImageView);
            origin.X = GrdDrawContainer.RenderTransform.Value.OffsetX;
            origin.Y = GrdDrawContainer.RenderTransform.Value.OffsetY;
            GrdDrawContainer.Cursor = Cursors.Hand;
        }

        private bool AllZoomPan {
            get { return OverlayCanvas.Selection.Count() <= 0; }
        }

        public void FitView() {
            FitView(Info.ImageWidth, Info.ImageHeight);
        }
        public void FitView(double imageWidth, double imageHeight) {
            //var fitZoom =ZoomUtil.CalculateFitZoom(ActualWidth, ActualHeight, imageWidth, imageHeight);
            var m = Matrix.Identity;
            //m.Scale(fitZoom, fitZoom);
            //m.OffsetX = (imageWidth - ActualWidth) /2.0;
            //m.OffsetY = (imageHeight - ActualHeight) /2.0;
            GrdDrawContainer.RenderTransform = new MatrixTransform(m);
        }

        Point startMove;
        Point origin;
        private void GrdDrawContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            ImageHost.ReleaseMouseCapture();
            OverlayCanvas.ClearSelection();
            GrdDrawContainer.Cursor = null;
        }

        public void UpdateImageSource(BitmapSource bitmapSource, bool newImageSize) {

            if (!IsLoaded || bitmapSource == null) {
                return;
            }
            this.ImageHost.Source = bitmapSource;
            if (!newImageSize) return;

            FitView(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
            UpdateOverlayCanvasSize(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
            FitView();

        }

        private void UpdateOverlayCanvasSize(double width, double height) {
            GrdImageView.Width = width;
            GrdImageView.Height = height;
            Info.ImageWidth = (int)width;
            Info.ImageHeight = (int)height;
            UpdateEffectiveScaleValue(GrdDrawContainer.RenderTransform.Value.M11);
        }

        private void UpdateEffectiveScaleValue(double scale) {
            OverlayCanvas.ActualScale = scale;
            Info.Zoom = scale;
            OverlayCanvas.RefreshClip();
        }

        private void GrdDrawContainer_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (!AllZoomPan) return;
            var p = e.MouseDevice.GetPosition(GrdDrawContainer);
            var m = GrdDrawContainer.RenderTransform.Value;
            if (e.Delta > 0)
                m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
            else
                m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);
            GrdDrawContainer.RenderTransform = new MatrixTransform(m);
            UpdateEffectiveScaleValue(m.M11);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e) {

        }
    }
}