using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfCanvasDrawing;
class ToolPointer : Tool {
    private enum SelectionMode {
        None,
        Move,
        Size,
        GroupSelection
    }

    private SelectionMode selectMode = SelectionMode.None;

    private GraphicsBase resizedObject;
    private int resizedObjectHandle;
    private Point lastPoint = new Point(0, 0);


    public ToolPointer() {
    }

    public override void OnMouseDown(DrawingCanvas drawingCanvas, MouseButtonEventArgs e) {

        Point point = e.GetPosition(drawingCanvas);
        selectMode = SelectionMode.None;

        GraphicsBase o;
        GraphicsBase movedObject = null;
        int handleNumber;

        // Test for resizing (only if control is selected, cursor is on the handle)
        for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--) {
            o = drawingCanvas[i];

            if (o.IsSelected) {
                handleNumber = o.MakeHitTest(point);

                if (handleNumber > 0) {
                    selectMode = SelectionMode.Size;

                    // keep resized object in class member
                    resizedObject = o;
                    resizedObjectHandle = handleNumber;

                    // Since we want to resize only one object, unselect all other objects
                    DrawingCanvasHelper.UnselectAll(drawingCanvas);
                    o.IsSelected = true;


                    break;
                }
            }
        }

        // Test for move (cursor is on the object)
        if (selectMode == SelectionMode.None) {
            for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--) {
                o = drawingCanvas[i];

                if (o.MakeHitTest(point) == 0) {
                    movedObject = o;
                    break;
                }
            }

            if (movedObject != null) {
                selectMode = SelectionMode.Move;

                // Unselect all if Ctrl is not pressed and clicked object is not selected yet
                if (Keyboard.Modifiers != ModifierKeys.Control && !movedObject.IsSelected) {
                    DrawingCanvasHelper.UnselectAll(drawingCanvas);
                }

                // Select clicked object
                movedObject.IsSelected = true;

                // Set move cursor
                drawingCanvas.Cursor = Cursors.SizeAll;

            }
        }

        // Click on background
        if (selectMode == SelectionMode.None) {
            // Unselect all if Ctrl is not pressed
            if (Keyboard.Modifiers != ModifierKeys.Control) {
                DrawingCanvasHelper.UnselectAll(drawingCanvas);
            }

            // Group selection. Create selection rectangle.
            var r = new GraphicsSelectionRectangle(
                point.X, point.Y,
                point.X + 1, point.Y + 1,
                drawingCanvas.ActualScale);

            r.Clip = new RectangleGeometry(new Rect(0, 0, drawingCanvas.ActualWidth, drawingCanvas.ActualHeight));
            drawingCanvas.GraphicsList.Add(r);
            selectMode = SelectionMode.GroupSelection;
        }


        lastPoint = point;

        // Capture mouse until MouseUp event is received
        drawingCanvas.CaptureMouse();
    }


    public override void OnMouseMove(DrawingCanvas drawingCanvas, MouseEventArgs e) {
        // Exclude all cases except left button on/off.
        if (e.MiddleButton == MouseButtonState.Pressed ||
             e.RightButton == MouseButtonState.Pressed) {
            drawingCanvas.Cursor = DrawingCanvasHelper.DefaultCursor;
            return;
        }

        Point point = e.GetPosition(drawingCanvas);

        // Set cursor when left button is not pressed
        if (e.LeftButton == MouseButtonState.Released) {
            Cursor cursor = null;

            for (int i = 0; i < drawingCanvas.Count; i++) {
                int n = drawingCanvas[i].MakeHitTest(point);

                if (n > 0) {
                    cursor = drawingCanvas[i].GetHandleCursor(n);
                    break;
                }
            }

            if (cursor == null)
                cursor = DrawingCanvasHelper.DefaultCursor;

            drawingCanvas.Cursor = cursor;

            return;

        }

        if (!drawingCanvas.IsMouseCaptured) {
            return;
        }


        // Find difference between previous and current position
        double dx = point.X - lastPoint.X;
        double dy = point.Y - lastPoint.Y;

        lastPoint = point;

        // Resize
        if (selectMode == SelectionMode.Size) {
            if (resizedObject != null) {
                resizedObject.MoveHandleTo(point, resizedObjectHandle);
            }
        }

        // Move
        if (selectMode == SelectionMode.Move) {
            foreach (GraphicsBase o in drawingCanvas.Selection) {
                o.Move(dx, dy);
                o.Normalize();
            }
        }

        // Group selection
        if (selectMode == SelectionMode.GroupSelection) {
            // Resize selection rectangle
            drawingCanvas[drawingCanvas.Count - 1].MoveHandleTo(
                point, 5);
        }
    }

    public override void OnMouseUp(DrawingCanvas drawingCanvas, MouseButtonEventArgs e) {
        if (!drawingCanvas.IsMouseCaptured) {
            drawingCanvas.Cursor = DrawingCanvasHelper.DefaultCursor;
            selectMode = SelectionMode.None;
            return;
        }

        if (resizedObject != null) {
            // after resizing
            resizedObject.Normalize();
            resizedObject = null;
        }

        if (selectMode == SelectionMode.GroupSelection) {
            GraphicsSelectionRectangle r = (GraphicsSelectionRectangle)drawingCanvas[drawingCanvas.Count - 1];
            r.Normalize();
            Rect rect = r.Rectangle;
            drawingCanvas.GraphicsList.Remove(r);
            foreach (GraphicsBase g in drawingCanvas.GraphicsList) {
                if (g.IntersectsWith(rect)) {
                    g.IsSelected = true;
                }
            }
        }

        drawingCanvas.ReleaseMouseCapture();
        drawingCanvas.Cursor = DrawingCanvasHelper.DefaultCursor;
        selectMode = SelectionMode.None;
    }

    public override void SetCursor(DrawingCanvas drawingCanvas) {
        drawingCanvas.Cursor = DrawingCanvasHelper.DefaultCursor;
    }


}