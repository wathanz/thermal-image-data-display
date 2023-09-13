using DrawToolsLib;
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
    public partial class ImageViewUI : UserControl {

        private ImageViewUIViewModel viewModel;
        public ImageViewUI() {
            InitializeComponent();
        }

        private void GrdImageView_MouseDown(object sender, MouseButtonEventArgs e) {

        }

        private void GrdImageView_SizeChanged(object sender, SizeChangedEventArgs e) {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            this.viewModel = new ImageViewUIViewModel();
            this.DataContext = viewModel;

            ImageHost.MouseLeftButtonDown += GrdDrawContainer_MouseLeftButtonDown;
            ImageHost.MouseLeftButtonUp += GrdDrawContainer_MouseLeftButtonUp;
            ImageHost.MouseMove += GrdDrawContainer_MouseMove;


        }

        private void AddGraphics() {

            OverlayCanvas.Clear();
            OverlayCanvas.GraphicsList.Add(new GraphicsRectangle(50, 30, 100, 60, 2, Colors.Blue, 1));
            OverlayCanvas.GraphicsList.Add(new GraphicsRectangle(150, 80, 200, 110, 2, Colors.Cyan, 1));
            OverlayCanvas.GraphicsList.Add(new GraphicsRectangle(230, 50, 280, 80, 2, Colors.Yellow, 1));
            UpdateOverlayCanvasSize();
        }

        public IEnumerable<GraphicsRectangle> GetRoiRectangles => OverlayCanvas.GetGraphics<GraphicsRectangle>();


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
            GrdDrawContainer.RenderTransform = Transform.Identity;
        }

        Point startMove;
        Point origin;
        private void GrdDrawContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            ImageHost.ReleaseMouseCapture();
            OverlayCanvas.ClearSelection();
            GrdDrawContainer.Cursor = null;


        }

        public void UpdateImageSource(BitmapSource bitmapSource, bool newImageSize) {

            if (!IsLoaded) {
                return;
            }
            this.ImageHost.Source = bitmapSource;
            if (!newImageSize) return;


            FitView();
            InvalidateVisual();
            AddGraphics();
            UpdateOverlayCanvasSize();

        }

        private void UpdateOverlayCanvasSize() {
            var imgSource = this.ImageHost.Source;
            if (imgSource == null) return;
            OverlayCanvas.Width = imgSource.Width;
            OverlayCanvas.Height = imgSource.Height;
            UpdateEffectiveScaleValue(GrdDrawContainer.RenderTransform.Value.M11);
        }

        private void UpdateEffectiveScaleValue(double scale) {
            OverlayCanvas.ActualScale = scale;
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