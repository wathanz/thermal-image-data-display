using System.Windows.Input;

namespace WpfCanvasDrawing;
public static class DrawingCanvasHelper {
    public static Cursor DefaultCursor {
        get {
            return Cursors.Arrow;
        }
    }

    public static void Add(DrawingCanvas drawingCanvas, GraphicsBase graphicsBase) {
        graphicsBase.UpdateCanvasSize(drawingCanvas.ActualWidth, drawingCanvas.ActualHeight);
        drawingCanvas.GraphicsList.Add(graphicsBase);
        graphicsBase.Normalize();
        drawingCanvas.RefreshClip();
    }

    public static void Remove(DrawingCanvas drawingCanvas, GraphicsBase graphicsBase) {
        var index = drawingCanvas.GraphicsList.IndexOf(graphicsBase);
        if (index < 0) return;
        drawingCanvas.GraphicsList.RemoveAt(index);
    }

    public static void SelectAll(DrawingCanvas drawingCanvas) {
        for (int i = 0; i < drawingCanvas.Count; i++) {
            drawingCanvas[i].IsSelected = true;
        }
    }

    public static void UnselectAll(DrawingCanvas drawingCanvas) {
        for (int i = 0; i < drawingCanvas.Count; i++) {
            drawingCanvas[i].IsSelected = false;
        }
    }

}