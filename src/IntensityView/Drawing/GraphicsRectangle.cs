using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;



namespace DrawToolsLib {
    public class GraphicsRectangle : GraphicsRectangleBase {
        private FormattedText formatedText;
        public GraphicsRectangle(double left, double top, double width, double height,
            double lineWidth, Color objectColor, double actualScale) {
            this.rectangleLeft = left;
            this.rectangleTop = top;
            this.rectangleRight = left + width;
            this.rectangleBottom = top + height;
            this.graphicsLineWidth = lineWidth;

            this.rectangleLeftOfCopy = left;
            this.rectangleTopOfCopy = top;
            this.rectangleRightOfCopy = rectangleRight;
            this.rectangleBottomOfCopy = rectangleBottom;

            this.graphicsLineWidth = lineWidth;
            this.graphicsObjectColor = objectColor;
            this.graphicsActualScale = actualScale;

            this.formatedText = new FormattedText("", CultureInfo.InvariantCulture,
                 FlowDirection.LeftToRight, defaultTypeface, 12, Brushes.Red, 96);
        }

        public GraphicsRectangle()
            :
            this(0.0, 0.0, 100.0, 100.0, 1.0, Colors.Black, 1.0) {
        }



        public override void Draw(DrawingContext drawingContext) {
            if (drawingContext == null) {
                throw new ArgumentNullException("drawingContext");
            }

            drawingContext.DrawRectangle(
                Brushes.Transparent,
                new Pen(new SolidColorBrush(ObjectColor), ActualLineWidth),
                Rectangle);

            if (!string.IsNullOrEmpty(DisplayText)) {
                drawingContext.DrawText(formatedText, new Point() { X = Left, Y = Bottom });
            }
            base.Draw(drawingContext);
        }


        public override bool Contains(Point point) {
            return this.Rectangle.Contains(point);
        }


        public override DataGraphicsBase CreateSerializedObject() {
            return new DataGraphicsRectangle(this);
        }

    }
}