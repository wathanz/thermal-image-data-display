using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace WpfCanvasDrawing;
public class GraphicsRoiRectangle : GraphicsRectangle {

    public int Width { get { return (int)Math.Abs(rectangleRight - rectangleLeft); } }
    public int Height { get { return (int)Math.Abs(rectangleTop - rectangleBottom); } }



    private string textOnTop = "";
    public string TextOnTop {
        get { return textOnTop; }
        set {
            if (textOnTop == value) return;
            textOnTop = value;
        }
    }

    private string textOnBottom = "";
    public string TextOnBottom {
        get { return textOnBottom; }
        set {

            if (textOnBottom == value) return;
            textOnBottom = value;
            RefreshDrawing();
        }
    }


    private string positionSizeInfo = "";
    public string PositionSizeInfo {
        get { return positionSizeInfo; }
        private set {
            if (positionSizeInfo == value) return;
            positionSizeInfo = value;
        }
    }

    private string colorString = "";
    public string ColorString {
        get { return colorString; }
        private set {

            if (colorString == value) return;
            colorString = value;
        }
    }


    public int DisplayTextFontSize {
        get {
            return displayTextFontSize;
        }

        set {
            if (value == displayTextFontSize) return;
            displayTextFontSize = value;
        }
    }
    protected FormattedText UpdateText(string text, Brush brush, double textSize) {
        return new FormattedText(text, CultureInfo.InvariantCulture,
             FlowDirection.LeftToRight, defaultTypeface, textSize, brush, 96);
    }

    public bool TrySetColorString(string newValue) {
        try {
            var color = (Color)ColorConverter.ConvertFromString(newValue);
            ObjectColor = color;
            return true;
        }
        //ignore conversion fail exception
        catch (Exception) {
        }
        return false;
    }

    int roiId;
    public int RoiId {
        get { return roiId; }
        set { roiId = value; }
    }

    private SolidColorBrush idTextColorBrush = Brushes.White;

    public GraphicsRoiRectangle(int counter,
        double left, double top, double width, double height,
        double lineWidth, Color objectColor, double actualScale) : base(left, top, width, height, lineWidth, objectColor, actualScale) {

        RoiId = counter;
        textOnTop = $"ROI-{counter}";
        colorString = ObjectColor.ToString();
        PositionSizeInfo = $"{Left:0}, {Top:0}, {Width:0}, {Height:0}";
        DrawingChanged += Rectangle_DrawingChanged;
    }

    private void UpdateTextContrastColor() {
        idTextColorBrush = CalculateGrayScale(ObjectColor) > 128 ? Brushes.Black : Brushes.White;
    }

    //conversion from RGB color to grayscale value
    private double CalculateGrayScale(Color c) {
        return 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
    }

    public override void Draw(DrawingContext drawingContext) {
        if (drawingContext == null) {
            throw new ArgumentNullException("drawingContext");
        }

        if (IsHidden) return;

        DrawText(drawingContext);
        base.Draw(drawingContext);
    }

    private void DrawText(DrawingContext drawingContext) {
        var drawText = !string.IsNullOrEmpty(textOnBottom) || !string.IsNullOrEmpty(textOnTop);
        if (!drawText) return;

        var actualFontSize = displayTextFontSize / graphicsActualScale;
        var textPadding = actualFontSize * 0.5;

        if (!string.IsNullOrEmpty(textOnBottom)) {
            var formattedText = UpdateText(textOnBottom, Brushes.White, actualFontSize);
            var textWidth = formattedText.Width + textPadding * 2;
            var textLeft = Right - textWidth;
            DrawTextWithBackground(drawingContext, actualFontSize, textPadding,
                textLeft, Bottom, formattedText, Brushes.Black);
        }

        if (!string.IsNullOrEmpty(textOnTop)) {
            UpdateTextContrastColor();
            var formattedText = UpdateText(textOnTop, idTextColorBrush, actualFontSize);
            var textPositionY = Top - actualFontSize;
            DrawTextWithBackground(drawingContext, actualFontSize, textPadding,
                Left, textPositionY, formattedText, new SolidColorBrush(ObjectColor));
        }
    }

    private void DrawTextWithBackground(DrawingContext drawingContext, double actualFontSize, double textPadding,
        double textPositionX, double textPositionY, FormattedText formattedText, SolidColorBrush backgroundBrush) {

        var textWidth = formattedText.Width + textPadding * 2;
        drawingContext.DrawRectangle(backgroundBrush, null, new Rect(textPositionX, textPositionY, textWidth, formattedText.Height));
        drawingContext.DrawText(formattedText, new Point() { X = textPositionX + textPadding, Y = textPositionY });
    }

    private void Rectangle_DrawingChanged(object sender, EventArgs e) {
        UpdateInfo();
    }

    private void UpdateInfo() {

        var rec = Rectangle;
        if (Threshold.IsEqual(Left, (int)rec.Left, 1) &&
            Threshold.IsEqual(Top, (int)rec.Top, 1) &&
            Threshold.IsEqual(Width, (int)rec.Width, 1) &&
            Threshold.IsEqual(Height, (int)rec.Height, 1))
            return;

        PositionSizeInfo = $"{Left:0}, {Top:0}, {Width:0}, {Height:0}";
        ColorString = ObjectColor.ToString();
    }
    ~GraphicsRoiRectangle() {
        DrawingChanged -= Rectangle_DrawingChanged;
    }
}