using System;
using System.Windows.Media;



namespace DrawToolsLib {
    public class DataGraphicsRectangle : DataGraphicsBase {
        private double left;
        private double top;
        private double right;
        private double bottom;
        private double lineWidth;
        private Color objectColor;

        public DataGraphicsRectangle() {

        }

        public DataGraphicsRectangle(GraphicsRectangle rectangle) {
            if (rectangle == null) {
                throw new ArgumentNullException("rectangle");
            }

            left = rectangle.Left;
            top = rectangle.Top;
            right = rectangle.Right;
            bottom = rectangle.Bottom;

            lineWidth = rectangle.LineWidth;
            objectColor = rectangle.ObjectColor;
            actualScale = rectangle.ActualScale;
            ID = rectangle.Id;
            selected = rectangle.IsSelected;
        }

        public override GraphicsBase CreateGraphics() {
            GraphicsBase b = new GraphicsRectangle(left, top, right, bottom, lineWidth, objectColor, actualScale);
            if (this.ID != 0) {
                b.Id = this.ID;
                b.IsSelected = this.selected;
            }
            return b;
        }

        public double Left {
            get { return left; }
            set { left = value; }
        }

        public double Top {
            get { return top; }
            set { top = value; }
        }

        public double Right {
            get { return right; }
            set { right = value; }
        }

        public double Bottom {
            get { return bottom; }
            set { bottom = value; }
        }

        public double LineWidth {
            get { return lineWidth; }
            set { lineWidth = value; }
        }

        public Color ObjectColor {
            get { return objectColor; }
            set { objectColor = value; }
        }

    }
}