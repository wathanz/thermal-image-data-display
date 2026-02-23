using System.Windows;
using System.Windows.Input;


namespace WpfCanvasDrawing;
public abstract class GraphicsRectangleBase : GraphicsBase {

    protected double rectangleLeft;
    protected double rectangleTop;
    protected double rectangleRight;
    protected double rectangleBottom;

    protected double rectangleLeftOfCopy;
    protected double rectangleTopOfCopy;
    protected double rectangleRightOfCopy;
    protected double rectangleBottomOfCopy;

    public Rect Rectangle {
        get {
            double l, t, w, h;

            if (rectangleLeft <= rectangleRight) {
                l = rectangleLeft;
                w = rectangleRight - rectangleLeft;
            }
            else {
                l = rectangleRight;
                w = rectangleLeft - rectangleRight;
            }

            if (rectangleTop <= rectangleBottom) {
                t = rectangleTop;
                h = rectangleBottom - rectangleTop;
            }
            else {
                t = rectangleBottom;
                h = rectangleTop - rectangleBottom;
            }

            return new Rect(l, t, w, h);
        }
    }



    public double Left {
        get { return rectangleLeft; }
        set { rectangleLeft = value; }
    }

    public double Top {
        get { return rectangleTop; }
        set { rectangleTop = value; }
    }

    public double Right {
        get { return rectangleRight; }
        set { rectangleRight = value; }
    }

    public double Bottom {
        get { return rectangleBottom; }
        set { rectangleBottom = value; }
    }

    public override int HandleCount {
        get {
            return 8;
        }
    }

    /// <summary>
    /// Get handle point by 1-based number
    /// </summary>
    public override Point GetHandle(int handleNumber) {
        double x, y, xCenter, yCenter;

        xCenter = (rectangleRight + rectangleLeft) / 2;
        yCenter = (rectangleBottom + rectangleTop) / 2;
        x = rectangleLeft;
        y = rectangleTop;

        switch (handleNumber) {
            case 1:
                x = rectangleLeft;
                y = rectangleTop;
                break;
            case 2:
                x = xCenter;
                y = rectangleTop;
                break;
            case 3:
                x = rectangleRight;
                y = rectangleTop;
                break;
            case 4:
                x = rectangleRight;
                y = yCenter;
                break;
            case 5:
                x = rectangleRight;
                y = rectangleBottom;
                break;
            case 6:
                x = xCenter;
                y = rectangleBottom;
                break;
            case 7:
                x = rectangleLeft;
                y = rectangleBottom;
                break;
            case 8:
                x = rectangleLeft;
                y = yCenter;
                break;
        }

        return new Point(x, y);
    }

    /// <summary>
    /// Hit test.
    /// Return value: -1 - no hit
    ///                0 - hit anywhere
    ///                > 1 - handle number
    /// </summary>
    public override int MakeHitTest(Point point) {
        if (IsSelected) {
            for (int i = 1; i <= HandleCount; i++) {
                if (GetHandleRectangle(i).Contains(point))
                    return i;
            }
        }

        if (Contains(point))
            return 0;

        return -1;
    }


    public override Cursor GetHandleCursor(int handleNumber) {
        switch (handleNumber) {
            case 1:
                return Cursors.SizeNWSE;
            case 2:
                return Cursors.SizeNS;
            case 3:
                return Cursors.SizeNESW;
            case 4:
                return Cursors.SizeWE;
            case 5:
                return Cursors.SizeNWSE;
            case 6:
                return Cursors.SizeNS;
            case 7:
                return Cursors.SizeNESW;
            case 8:
                return Cursors.SizeWE;
            default:
                return DrawingCanvasHelper.DefaultCursor;
        }
    }

    public override void MoveHandleTo(Point point, int handleNumber) {
        switch (handleNumber) {
            case 1:
                rectangleLeft = point.X;
                rectangleTop = point.Y;
                break;
            case 2:
                rectangleTop = point.Y;
                break;
            case 3:
                rectangleRight = point.X;
                rectangleTop = point.Y;
                break;
            case 4:
                rectangleRight = point.X;
                break;
            case 5:
                rectangleRight = point.X;
                rectangleBottom = point.Y;
                break;
            case 6:
                rectangleBottom = point.Y;
                break;
            case 7:
                rectangleLeft = point.X;
                rectangleBottom = point.Y;
                break;
            case 8:
                rectangleLeft = point.X;
                break;
        }

        RefreshDrawing();
    }

    public override bool IntersectsWith(Rect rectangle) {
        return Rectangle.IntersectsWith(rectangle);
    }


    public override void Move(double deltaX, double deltaY) {
        rectangleLeft += deltaX;
        rectangleRight += deltaX;

        rectangleTop += deltaY;
        rectangleBottom += deltaY;

    }

    public override void Normalize() {

        if (rectangleLeft > rectangleRight) {
            double tmp = rectangleLeft;
            rectangleLeft = rectangleRight;
            rectangleRight = tmp;
        }

        if (rectangleTop > rectangleBottom) {
            double tmp = rectangleTop;
            rectangleTop = rectangleBottom;
            rectangleBottom = tmp;
        }

        var w = (int)canvasWidth;
        var h = (int)canvasHeight;

        if (rectangleLeft < 0) rectangleLeft = 0;
        else if (rectangleLeft > w - 1) rectangleLeft = w - 1;
        if (rectangleTop < 0) rectangleTop = 0;
        else if (rectangleTop > h - 1) rectangleTop = h - 1;

        if (rectangleLeft == rectangleRight) rectangleRight += 1;
        if (rectangleTop == rectangleBottom) rectangleBottom += 1;

        if (rectangleRight < 1) rectangleRight = 1;
        else if (rectangleRight > w) rectangleRight = w;
        if (rectangleBottom < 1) rectangleBottom = 1;
        else if (rectangleBottom > h) rectangleBottom = h;

        RefreshDrawing();
    }
}