using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace WpfCanvasDrawing;
public abstract class GraphicsBase : DrawingVisual {



    protected const double HitTestWidth = 8.0;
    protected const double HandleSize = 8;


    static SolidColorBrush handleBrush1 = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
    static SolidColorBrush handleBrush2 = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
    static SolidColorBrush handleBrush3 = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));

    protected static Typeface defaultTypeface = new Typeface("Consolas");
    protected FormattedText formatedText;
    protected int displayTextFontSize = 11;
    protected double canvasWidth = 0;
    protected double canvasHeight = 0;
    public event EventHandler DrawingChanged;
    protected GraphicsBase() {
        objectId = this.GetHashCode();
    }

    protected bool isHidden;
    public bool IsHidden {
        get {
            return isHidden;
        }
        set {
            if (isHidden == value) return;
            isHidden = value;
            RefreshDrawing();
        }
    }


    protected bool isSelected;
    public bool IsSelected {
        get {
            return isSelected;
        }
        set {
            if (isSelected == value) return;
            isSelected = value;
            RefreshDrawing();
        }
    }


    protected double lineWidth;
    public double LineWidth {
        get {
            return lineWidth;
        }

        set {

            if (lineWidth == value) return;
            lineWidth = value;
            RefreshDrawing();
        }
    }

    protected Color graphicsObjectColor;
    public Color ObjectColor {
        get {
            return graphicsObjectColor;
        }

        set {
            graphicsObjectColor = value;
            RefreshDrawing();
        }
    }

    protected double graphicsActualScale;
    public double ActualScale {
        get {
            return graphicsActualScale;
        }

        set {
            if (graphicsActualScale == value) return;
            graphicsActualScale = value;
            RefreshDrawing();
        }
    }
    protected double ActualLineWidth {
        get {
            return graphicsActualScale <= 0 ? lineWidth : lineWidth / graphicsActualScale;
        }
    }


    protected double LineHitTestWidth {
        get {
            return Math.Max(HitTestWidth, ActualLineWidth);
        }
    }

    public void UpdateCanvasSize(double width, double height) {
        canvasWidth = width;
        canvasHeight = height;
    }

    int objectId;
    public int Id {
        get { return objectId; }
        set { objectId = value; }
    }

    public abstract int HandleCount {
        get;
    }



    public abstract bool Contains(Point point);
    public abstract Point GetHandle(int handleNumber);
    public abstract int MakeHitTest(Point point);
    public abstract bool IntersectsWith(Rect rectangle);
    public abstract void Move(double deltaX, double deltaY);
    public abstract void MoveHandleTo(Point point, int handleNumber);
    public abstract Cursor GetHandleCursor(int handleNumber);
    public virtual void Normalize() {
    }

    public virtual void Draw(DrawingContext drawingContext) {
        if (isSelected && !isHidden) {
            DrawTracker(drawingContext);
        }
        if (DrawingChanged != null) {
            DrawingChanged(this, new EventArgs());
        }
    }

    public virtual void DrawTracker(DrawingContext drawingContext) {
        for (int i = 1; i <= HandleCount; i++) {
            DrawTrackerRectangle(drawingContext, GetHandleRectangle(i));
        }
    }

    [Conditional("DEBUG")]
    public virtual void Dump() {
        Trace.WriteLine(this.GetType().Name);
        Trace.WriteLine($"ID ={objectId} Selected={isSelected}");
        Trace.WriteLine($"objectColor ={ColorToDisplay(graphicsObjectColor)} " +
            $"lineWidth = {DoubleForDisplay(lineWidth)}");
    }

    private void DrawTrackerRectangle(DrawingContext drawingContext, Rect rectangle) {
        // External rectangle
        drawingContext.DrawRectangle(handleBrush1, null, rectangle);

        // Middle
        drawingContext.DrawRectangle(handleBrush2, null,
            new Rect(rectangle.Left + rectangle.Width / 8,
                     rectangle.Top + rectangle.Height / 8,
                     rectangle.Width * 6 / 8,
                     rectangle.Height * 6 / 8));

        // Internal
        drawingContext.DrawRectangle(new SolidColorBrush(graphicsObjectColor), null,
            new Rect(rectangle.Left + rectangle.Width / 4,
             rectangle.Top + rectangle.Height / 4,
             rectangle.Width / 2,
             rectangle.Height / 2));
    }


    public void RefreshDrawing() {
        var dc = this.RenderOpen();
        Draw(dc);
        dc.Close();
    }

    public Rect GetHandleRectangle(int handleNumber) {
        var point = GetHandle(handleNumber);
        double size = Math.Max(HandleSize / graphicsActualScale, ActualLineWidth * 1.1);
        return new Rect(point.X - size / 2, point.Y - size / 2,
            size, size);
    }

    static string DoubleForDisplay(double value) {
        return ((float)value).ToString("f2", CultureInfo.InvariantCulture);
    }

    static string ColorToDisplay(Color value) {
        return "R:" + value.R.ToString(CultureInfo.InvariantCulture) +
               " G:" + value.G.ToString(CultureInfo.InvariantCulture) +
               " B:" + value.B.ToString(CultureInfo.InvariantCulture);
    }

}