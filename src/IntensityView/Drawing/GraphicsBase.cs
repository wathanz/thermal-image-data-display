using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace DrawingLib {
    public abstract class GraphicsBase : DrawingVisual {

        protected double graphicsLineWidth;
        protected Color graphicsObjectColor;

        protected double graphicsActualScale;
        protected bool selected;
        protected bool isHide;
        private bool isSigned = false;

        private bool option;

        int objectId;

        protected const double HitTestWidth = 8.0;
        protected const double HandleSize = 8;


        static SolidColorBrush handleBrush1 = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        static SolidColorBrush handleBrush2 = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        static SolidColorBrush handleBrush3 = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));

        protected static Typeface defaultTypeface = new Typeface("Tahoma");
        protected FormattedText formatedText;
        protected string displayText = "";
        protected int displayTextFontSize = 12;

        public event EventHandler DrawingChanged;
        protected GraphicsBase() {
            objectId = this.GetHashCode();
        }


        public bool IsHide {
            get {
                return isHide;
            }
            set {
                if (isHide == value) return;
                isHide = value;
                RefreshDrawing();
            }
        }
        public bool IsSelected {
            get {
                return selected;
            }
            set {
                if (selected == value) return;
                selected = value;
                RefreshDrawing();
            }
        }

        public bool IsOption {
            get {
                return option;
            }
            set {
                if (option == value) return;
                option = value;
                RefreshDrawing();
            }
        }

        public double LineWidth {
            get {
                return graphicsLineWidth;
            }

            set {

                if (graphicsLineWidth == value) return;
                graphicsLineWidth = value;
                RefreshDrawing();
            }
        }

        public Color ObjectColor {
            get {
                return graphicsObjectColor;
            }

            set {
                graphicsObjectColor = value;
                RefreshDrawing();
            }
        }

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


        public string DisplayText {
            get {
                return displayText;
            }

            set {
                if (value == displayText) return;
                displayText = value;
                UpdateDisplayText();
            }
        }


        public int DisplayTextFontSize {
            get {
                return displayTextFontSize;
            }

            set {
                if (value == displayTextFontSize) return;
                displayTextFontSize = value;
                UpdateDisplayText();
            }
        }

        private void UpdateDisplayText() {
            this.formatedText = new FormattedText(displayText, CultureInfo.InvariantCulture,
                 FlowDirection.LeftToRight, defaultTypeface, displayTextFontSize, Brushes.Red, 96);


            RefreshDrawing();
        }
        protected double ActualLineWidth {
            get {
                return graphicsActualScale <= 0 ? graphicsLineWidth : graphicsLineWidth / graphicsActualScale;
            }
        }


        protected double LineHitTestWidth {
            get {
                return Math.Max(8.0, ActualLineWidth);
            }
        }
        public int Id {
            get { return objectId; }
            set { objectId = value; }
        }

        public abstract int HandleCount {
            get;
        }
        public bool IsSigned { get => isSigned; set => isSigned = value; }


        public abstract bool Contains(Point point);
        public abstract DataGraphicsBase CreateSerializedObject();
        public abstract Point GetHandle(int handleNumber);
        public abstract int MakeHitTest(Point point);
        public abstract bool IntersectsWith(Rect rectangle);
        public abstract void Move(double deltaX, double deltaY);
        public abstract void Zoom(double scale, Point center);
        public abstract void CopyPoints();
        public abstract void Reset(double scale, Point center);
        public abstract void MoveHandleTo(Point point, int handleNumber);
        public abstract Cursor GetHandleCursor(int handleNumber);
        public virtual void Normalize() {
        }

        public virtual void Draw(DrawingContext drawingContext) {
            if (IsSelected) {
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

            Trace.WriteLine("ID = " + objectId.ToString(CultureInfo.InvariantCulture) +
                "   Selected = " + selected.ToString(CultureInfo.InvariantCulture));

            Trace.WriteLine("objectColor = " + ColorToDisplay(graphicsObjectColor) +
                "  lineWidth = " + DoubleForDisplay(graphicsLineWidth));
        }

        static void DrawTrackerRectangle(DrawingContext drawingContext, Rect rectangle) {
            // External rectangle
            drawingContext.DrawRectangle(handleBrush1, null, rectangle);

            // Middle
            drawingContext.DrawRectangle(handleBrush2, null,
                new Rect(rectangle.Left + rectangle.Width / 8,
                         rectangle.Top + rectangle.Height / 8,
                         rectangle.Width * 6 / 8,
                         rectangle.Height * 6 / 8));

            // Internal
            drawingContext.DrawRectangle(handleBrush3, null,
                new Rect(rectangle.Left + rectangle.Width / 4,
                 rectangle.Top + rectangle.Height / 4,
                 rectangle.Width / 2,
                 rectangle.Height / 2));
        }


        public void RefreshDrawing() {
            DrawingContext dc = this.RenderOpen();
            Draw(dc);
            dc.Close();
        }

        public Rect GetHandleRectangle(int handleNumber) {
            Point point = GetHandle(handleNumber);

            double size = Math.Max(HandleSize / graphicsActualScale, ActualLineWidth * 1.1);
            return new Rect(point.X - size / 2, point.Y - size / 2,
                size, size);
        }

        static string DoubleForDisplay(double value) {
            return ((float)value).ToString("f2", CultureInfo.InvariantCulture);
        }

        static string ColorToDisplay(Color value) {
            //return "A:" + value.A.ToString() +
            return "R:" + value.R.ToString(CultureInfo.InvariantCulture) +
                   " G:" + value.G.ToString(CultureInfo.InvariantCulture) +
                   " B:" + value.B.ToString(CultureInfo.InvariantCulture);
        }

    }
}