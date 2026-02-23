namespace WpfCanvasDrawing;
public static class MathHelper {
    public static bool CheckIfInbetween(int value, int min, int max) {
        return value >= min && value <= max;
    }
}