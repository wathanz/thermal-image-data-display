using DrawingLib;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;



namespace DrawToolsLib {
    abstract class ToolObject : Tool {
        private Cursor toolCursor;

        protected Cursor ToolCursor {
            get {
                return toolCursor;
            }
            set {
                toolCursor = value;
            }
        }


        public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e) {
            if (drawingCanvas.Count > 0) {

                drawingCanvas[drawingCanvas.Count - 1].CopyPoints();
                drawingCanvas[drawingCanvas.Count - 1].IsSigned = true;
                drawingCanvas[drawingCanvas.Count - 1].Normalize();
            }

            drawingCanvas.ReleaseMouseCapture();
        }

        protected static void AddNewObject(DrawingCanvas drawingCanvas, GraphicsBase o) {
            HelperFunctions.UnselectAll(drawingCanvas);

            o.IsSelected = true;
            o.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));

            drawingCanvas.GraphicsList.Add(o);
            drawingCanvas.CaptureMouse();
        }

        public override void SetCursor(DrawingCanvas drawingCanvas) {
            drawingCanvas.Cursor = this.toolCursor;
        }

    }
}