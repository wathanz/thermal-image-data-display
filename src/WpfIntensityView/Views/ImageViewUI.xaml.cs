using IntensityMapping.Core;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfCanvasDrawing;
using WpfIntensityView.Interface;
using WpfIntensityView.Model;

namespace WpfIntensityView.Views;
/// <summary>
/// Interaction logic for ImageViewUI.xaml
/// </summary>
public partial class ImageViewUI : UserControl, IImageViewUI {
    public ImageViewUI() {
        InitializeComponent();
        Info = new ImageViewInfo();
    }

    private void GrdImageView_MouseDown(object sender, MouseButtonEventArgs e) {

    }

    private void GrdImageView_SizeChanged(object sender, SizeChangedEventArgs e) {

    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        DataContext = Info;
        ImageHost.MouseLeftButtonDown += GrdDrawContainer_MouseLeftButtonDown;
        ImageHost.MouseLeftButtonUp += GrdDrawContainer_MouseLeftButtonUp;
        ImageHost.MouseMove += GrdDrawContainer_MouseMove;
    }


    public ImageViewInfo Info { get; }

    static int roiCount = 0;
    public IEnumerable<GraphicsRoiRectangle> GetRoiRectangles() {
        var list = new List<GraphicsRoiRectangle>();
        foreach (var item in OverlayCanvas.GetGraphics<GraphicsRoiRectangle>()) {
            list.Add(item);
        }
        return list;
    }

    private static Color[] RoiColors = new[] {
        Colors.Blue, Colors.Red, Color.FromRgb(0,255,0), Colors.Cyan, Colors.Magenta, Colors.Pink, Colors.Yellow
    };
    public void AddNewRoi(int left = 5, int top = 5, int width = 50, int height = 30) {
        var limit = 20;
        if (OverlayCanvas.Count >= limit) throw new DrawingCanvasException($"Roi Max Limit {limit}");


        var colorIndex = roiCount % RoiColors.Length;
        var roiId = ++roiCount;
        var addedRoiRectangle = new GraphicsRoiRectangle(roiId, left, top, width, height, 1, RoiColors[colorIndex], OverlayCanvas.ActualScale);
        DrawingCanvasHelper.Add(OverlayCanvas, addedRoiRectangle);
    }
    public void RemoveRoi(GraphicsRoiRectangle roi) {
        if (roi == null) return;
        DrawingCanvasHelper.Remove(OverlayCanvas, roi);
    }

    private void GrdDrawContainer_MouseMove(object sender, MouseEventArgs e) {
        if (!ImageHost.IsMouseCaptured) return;
        var currentPoint = e.MouseDevice.GetPosition(GrdDrawContainer);
        var matrix = ImageHost.RenderTransform.Value;
        var deltaX = currentPoint.X - startPoint.X;
        var deltaY = currentPoint.Y - startPoint.Y;
        matrix.OffsetX = origin.X + deltaX;
        matrix.OffsetY = origin.Y + deltaY;
        SetRenderTransform(matrix);

    }

    private void SetRenderTransform(Matrix m) {
        var matrixTransform = new MatrixTransform(m);
        ImageHost.RenderTransform = matrixTransform;
        OverlayCanvas.RenderTransform = matrixTransform;
        UpdateEffectiveScaleValue(matrixTransform.Value.M11);
    }

    private void GrdDrawContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
        if (!AllZoomPan || this.OverlayCanvas.Tool != ToolType.Pointer) return;
        startPoint = e.GetPosition(GrdDrawContainer);
        origin.X = ImageHost.RenderTransform.Value.OffsetX;
        origin.Y = ImageHost.RenderTransform.Value.OffsetY;
        ImageHost.Cursor = Cursors.Hand;
        ImageHost.CaptureMouse();
    }

    private bool AllZoomPan {
        get { return OverlayCanvas.Selection.Count() <= 0; }
    }

    public void FitView() {
        FitView(Info.ImageWidth, Info.ImageHeight);
    }
    public void FitView(double imageWidth, double imageHeight) {
        var fitZoom = ZoomUtil.CalculateFitZoom(ActualWidth, ActualHeight, imageWidth, imageHeight);
        var matrix = Matrix.Identity;
        matrix.Scale(fitZoom, fitZoom);
        matrix.OffsetX = (ActualWidth - imageWidth * fitZoom) / 2f;
        matrix.OffsetY = (ActualHeight - imageHeight * fitZoom) / 2f;
        SetRenderTransform(matrix);
    }

    Point startPoint;
    Point origin;
    private void GrdDrawContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
        ClearSelection();
    }

    private void ClearSelection() {
        ImageHost.ReleaseMouseCapture();
        OverlayCanvas.ClearSelection();
        ImageHost.Cursor = null;
    }

    public void UpdateImageSource(BitmapSource bitmapSource, bool newImageSize, FpsTrackingInfo trackingInfo) {

        if (!IsLoaded || bitmapSource == null) {
            return;
        }

        ImageHost.Source = bitmapSource;
        if (trackingInfo != null) {
            Info.Fps = trackingInfo.Fps;
        }

        if (!newImageSize) return;
        FitView(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
        UpdateOverlayCanvasSize(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
        FitView();

    }

    private void UpdateOverlayCanvasSize(double width, double height) {
        GrdDrawContainer.Width = width;
        GrdDrawContainer.Height = height;
        Info.ImageWidth = (int)width;
        Info.ImageHeight = (int)height;
    }

    private void UpdateEffectiveScaleValue(double scale) {
        OverlayCanvas.ActualScale = scale;
        Info.Zoom = scale;
        OverlayCanvas.RefreshClip();
    }

    private void GrdDrawContainer_MouseWheel(object sender, MouseWheelEventArgs e) {
        if (!AllZoomPan) return;
        var mousePosition = e.MouseDevice.GetPosition(ImageHost);
        var renderMatrix = ImageHost.RenderTransform.Value;
        var scaleValue = e.Delta > 0 ? 1.1 : 1.0 / 1.1;
        renderMatrix.ScaleAtPrepend(scaleValue, scaleValue, mousePosition.X, mousePosition.Y);
        SetRenderTransform(renderMatrix);
    }

}