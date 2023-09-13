using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace DrawingLib {
    static class HelperFunctions {
        public static bool hasSelected(DrawingCanvas drawingCanvas) {
            bool ok = false;
            foreach (GraphicsBase g in drawingCanvas.GraphicsList) {
                if (g.IsOption) {
                    ok = true;
                    return ok;
                }
            }
            return ok;
        }

        public static double calculateCos(Point from1, Point to1, Point from2, Point to2) {
            double x1 = to1.X - from1.X;
            double y1 = to1.Y - from1.Y;
            double x2 = to2.X - from2.X;
            double y2 = to2.Y - from2.Y;

            double numerator = x1 * x2 + y1 * y2;
            double denominator = Math.Sqrt(x1 * x1 + y1 * y1) * Math.Sqrt(x2 * x2 + y2 * y2);

            double ans = numerator / denominator;

            if (Math.Abs(ans) < 1 + Threshold.Esp) {
                return ans;
            }
            return double.MaxValue;
        }


        public static Point CalculateCenterPoint(Point p1, Point p2) {
            double cx = (p1.X + p2.X) / 2;
            double cy = (p1.Y + p2.Y) / 2;
            Point centerPoint = new Point(cx, cy);
            return centerPoint;
        }

        public static int CalcDistanceSquare(Point p1, Point p2) {
            return (int)((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static Cursor DefaultCursor {
            get {
                return Cursors.Arrow;
            }
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
}