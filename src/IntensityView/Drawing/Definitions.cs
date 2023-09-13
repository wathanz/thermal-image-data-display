using System;

namespace DrawingLib {
    public enum ToolType {
        None,
        Pointer,
        Rectangle,
        Max
    };

    public static class Threshold {
        const double esp = 1E-5;
        public static double Esp => esp;

        public static bool IsEqual(double a, double b, double diff = esp) {
            return Math.Abs(a - b) < diff;
        }
    }

}