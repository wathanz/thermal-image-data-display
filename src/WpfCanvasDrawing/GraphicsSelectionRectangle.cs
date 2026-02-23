using System.Windows;
using System.Windows.Media;


namespace WpfCanvasDrawing;
class GraphicsSelectionRectangle : GraphicsRectangleBase {

    public GraphicsSelectionRectangle(double left, double top, double right, double bottom, double actualScale) {
        rectangleLeft = left;
        rectangleTop = top;
        rectangleRight = right;
        rectangleBottom = bottom;
        lineWidth = 1.0;
        graphicsActualScale = actualScale;
    }

    public GraphicsSelectionRectangle()
        :
        this(0.0, 0.0, 100.0, 100.0, 1.0) {
    }


    public override void Draw(DrawingContext drawingContext) {
        drawingContext.DrawRectangle(
            null,
            new Pen(Brushes.White, ActualLineWidth),
            Rectangle);

        var dashStyle = new DashStyle();
        dashStyle.Dashes.Add(4);

        var dashedPen = new Pen(Brushes.Black, ActualLineWidth);
        dashedPen.DashStyle = dashStyle;


        drawingContext.DrawRectangle(
            null,
            dashedPen,
            Rectangle);
    }

    public override bool Contains(Point point) {
        return Rectangle.Contains(point);
    }

}