using System.Windows.Input;

namespace WpfCanvasDrawing.Tools;
abstract class ToolObject : Tool {
    public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e) {
        if (drawingCanvas.Count > 0) {
            var graphicsBase = drawingCanvas[drawingCanvas.Count - 1];
            graphicsBase.Normalize();
        }
        drawingCanvas.ReleaseMouseCapture();
    }
}