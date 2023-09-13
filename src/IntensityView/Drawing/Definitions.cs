namespace DrawToolsLib {
    public enum ToolType {
        None,
        Pointer,
        Rectangle,
        Max
    };
    public enum GestureId {
        None,
        ZOOM,
        PAN,
        RORATE,
        ERASE
    };

    public static class Threshold {
        const int panMaxDist = 5000;
        const int scaleThreshold = 2;
        const double esp = 1E-5;
        const double inf = 1E+14;

        public static int ScaleThreshold => scaleThreshold;

        public static double Esp => esp;

        public static double Inf => inf;

        public static int PanMaxDist => panMaxDist;
    }

}