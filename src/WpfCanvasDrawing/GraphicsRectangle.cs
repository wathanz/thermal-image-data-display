using System;
using System.Windows;
using System.Windows.Media;



namespace WpfCanvasDrawing;
public class GraphicsRectangle : GraphicsRectangleBase {
    public GraphicsRectangle(double left, double top, double width, double height,
        double lineWidth, Color objectColor, double actualScale) {

        this.lineWidth = lineWidth;

        rectangleLeft = left;
        rectangleTop = top;
        rectangleRight = left + width;
        rectangleBottom = top + height;

        rectangleLeftOfCopy = left;
        rectangleTopOfCopy = top;
        rectangleRightOfCopy = rectangleRight;
        rectangleBottomOfCopy = rectangleBottom;

        graphicsObjectColor = objectColor;
        graphicsActualScale = actualScale;


    }

    public GraphicsRectangle()
        :
        this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0) {
    }



    public override void Draw(DrawingContext drawingContext) {
        if (drawingContext == null) {
            throw new ArgumentNullException("drawingContext");
        }
        if (IsHidden) return;

        drawingContext.DrawRectangle(
            Brushes.Transparent,
            new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth),
            Rectangle);


        base.Draw(drawingContext);
    }


    public override bool Contains(Point point) {
        return this.Rectangle.Contains(point);
    }



}