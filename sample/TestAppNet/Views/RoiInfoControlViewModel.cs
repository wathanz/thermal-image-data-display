using IntensityMapping.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfCanvasDrawing;

namespace DemoAppNet.Views;
internal class RoiInfoControlViewModel : NotifyPropertyChangedBase, IDataErrorInfo {
    private readonly GraphicsRoiRectangle roiRectangle;
    public int Id => roiRectangle.Id;

    int left = 0;
    public int Left {
        get { return left; }
        set {
            SetProperty(ref left, value);
        }
    }

    int top = 0;
    public int Top {
        get { return top; }
        set {
            SetProperty(ref top, value);
        }
    }

    string topEntry = "";
    public string TopEntry {
        get { return topEntry; }
        set {
            SetProperty(ref topEntry, value);
        }
    }

    string leftEntry = "";
    public string LeftEntry {
        get { return leftEntry; }
        set {
            SetProperty(ref leftEntry, value);
        }
    }

    int width = 0;
    public int Width {
        get { return width; }
        set {
            SetProperty(ref width, value);
        }
    }

    string widthEntry = "";
    public string WidthEntry {
        get { return widthEntry; }
        set {
            SetProperty(ref widthEntry, value);
        }
    }

    int height = 0;
    public int Height {
        get { return height; }
        set {
            SetProperty(ref height, value);
        }
    }

    string heightEntry = "";
    public string HeightEntry {
        get { return heightEntry; }
        set {
            SetProperty(ref heightEntry, value);
        }
    }


    int thickness = 0;
    public int Thickness {
        get { return thickness; }
        set {
            SetProperty(ref thickness, value);
        }
    }

    string thicknessEntry = "";
    public string ThicknessEntry {
        get { return thicknessEntry; }
        set {
            SetProperty(ref thicknessEntry, value);
        }
    }

    string txtColorEntry = "";
    public string TxtColorEntry {
        get { return txtColorEntry; }
        set {
            SetProperty(ref txtColorEntry, value);
        }
    }

    int counterId = 0;
    public int CounterId {
        get { return counterId; }
    }

    string textOnTop = "";
    public string TextOnTop {
        get { return textOnTop; }
    }

    string textOnBottom = "";
    public string TextOnBottom {
        get { return textOnBottom; }
    }

    bool isVisible = true;
    public bool IsVisible {
        get { return isVisible; }
        set {
            if (!SetProperty(ref isVisible, value)) return;
            this.roiRectangle.IsHidden = !isVisible;
        }
    }

    private IDictionary<string, string> errorCollection = new Dictionary<string, string>();
    public string Error {
        get {
            if (errorCollection.Count <= 0) return null;
            return string.Join(Environment.NewLine, errorCollection);
        }

    }
    private string GetByName(string name) {
        return GetType().GetProperty(name).GetValue(this).ToString();
    }
    public string this[string columnName] {
        get {

            var nameValue = GetByName(columnName);
            var valid = true; string errorMsg = "";
            if (IntValueEntryNames.Contains(columnName)) {
                valid = ValidateIfInteger(nameValue, out int value, out errorMsg) &&
                        ValidateIntegarValueEntry(columnName, value, out errorMsg);
            }
            else if (TextValueEntryNames.Contains(columnName)) {
                valid = ValidateTextValueEntry(columnName, nameValue, out errorMsg);
            }

            var contain = errorCollection.ContainsKey(columnName);
            if (valid && contain) {
                errorCollection.Remove(columnName);
            }
            else if (!valid && !contain) {
                errorCollection.Add(columnName, errorMsg);
            }
            return errorMsg;
        }
    }

    private bool ValidateIntegarValueEntry(string columnName, int value, out string errorMsg) {
        errorMsg = "";
        switch (columnName) {
            case nameof(HeightEntry):
                errorMsg = ValidateHeightEntry(value);
                break;
            case nameof(WidthEntry):
                errorMsg = ValidateWidthEntry(value);
                break;
            case nameof(TopEntry):
                errorMsg = ValidateTopEntry(value);
                break;
            case nameof(LeftEntry):
                errorMsg = ValidateLeftEntry(value);
                break;
            case nameof(ThicknessEntry):
                errorMsg = ValidateThicknessEntry(value);
                break;
            default:
                break;
        }
        return string.IsNullOrEmpty(errorMsg);
    }

    private bool ValidateTextValueEntry(string columnName, string value, out string errorMsg) {
        errorMsg = "";
        switch (columnName) {
            case nameof(TxtColorEntry):
                errorMsg = ValidateColorEntry(value);
                break;
            default:
                break;
        }
        return string.IsNullOrEmpty(errorMsg);
    }

    private bool ValidateIfInteger(string entry, out int result, out string errMsg) {
        errMsg = "";
        if (!int.TryParse(entry, out result)) {
            errMsg = "not integer value";
            return false;
        }
        return true;
    }
    private string ValidateColorEntry(string input) {
        try {
            _ = (Color)ColorConverter.ConvertFromString(input);
        }
        catch (Exception ex) {
            return ex.Message;
        }
        return "";
    }

    private string ValidateThicknessEntry(int result) {
        var errMsg = "";
        do {

            if (!MathHelper.CheckIfInbetween(result, 1, (int)10)) {
                errMsg = "invalid thickness value 1~10";
                break;
            }
        }
        while (false);
        return errMsg;
    }
    private string ValidateLeftEntry(int result) {
        var errMsg = "";
        do {

            if (!MathHelper.CheckIfInbetween(result, 0, (int)imageSize.Width)) {
                errMsg = "larger than image width";
                break;
            }

            var newRight = result + Width;
            if (!MathHelper.CheckIfInbetween(newRight, 0, (int)imageSize.Width)) {
                errMsg = "right is out of image width boundary";
                break;
            }
        }
        while (false);
        return errMsg;
    }

    private string ValidateTopEntry(int result) {
        var errMsg = "";
        do {

            if (!MathHelper.CheckIfInbetween(result, 0, (int)imageSize.Height)) {
                errMsg = "larger than image height";
                break;
            }

            var newRight = result + Height;
            if (!MathHelper.CheckIfInbetween(newRight, 0, (int)imageSize.Height)) {
                errMsg = "right is out of image height boundary";
                break;
            }
        }
        while (false);
        return errMsg;
    }
    private string ValidateWidthEntry(int result) {
        var errMsg = "";
        do {

            if (!MathHelper.CheckIfInbetween(result, 0, (int)imageSize.Width)) {
                errMsg = "larger than image width";
                break;
            }

            var newRight = result + Left;
            if (!MathHelper.CheckIfInbetween(newRight, 0, (int)imageSize.Width)) {
                errMsg = "right is out of image width boundary";
                break;
            }
        }
        while (false);
        return errMsg;
    }
    private string ValidateHeightEntry(int result) {
        var errMsg = "";
        do {

            if (!MathHelper.CheckIfInbetween(result, 0, (int)imageSize.Height)) {
                errMsg = "larger than image height";
                break;
            }

            var newBottom = result + Top;
            if (!MathHelper.CheckIfInbetween(newBottom, 0, (int)imageSize.Height)) {
                errMsg = "bottom is out of image height boundary";
                break;
            }
        }
        while (false);
        return errMsg;
    }

    private Size imageSize;
    public RoiInfoControlViewModel(GraphicsRoiRectangle graphicsRoiRectangle, Size size) {
        this.roiRectangle = graphicsRoiRectangle;
        this.roiRectangle.DrawingChanged += GraphicsRoiRectangle_DrawingChanged;
        IsVisible = !graphicsRoiRectangle.IsHidden;
        counterId = graphicsRoiRectangle.RoiId;
        textOnTop = graphicsRoiRectangle.TextOnTop;
        textOnBottom = graphicsRoiRectangle.TextOnBottom;
        txtColorEntry = graphicsRoiRectangle.ColorString;
        imageSize = size;
        UpdatePositionFromDrawing();

    }


    private void GraphicsRoiRectangle_DrawingChanged(object sender, EventArgs e) {

        UpdatePositionFromDrawing();
    }

    public void UpdateRoiDrawing() {
        roiRectangle.Left = left;
        roiRectangle.Top = top;
        roiRectangle.Right = left + width;
        roiRectangle.Bottom = top + height;
        roiRectangle.Normalize();
    }

    private string[] IntValueEntryNames = { "TopEntry", "LeftEntry", "HeightEntry", "WidthEntry", "ThicknessEntry" };
    private string[] TextValueEntryNames = { "TxtColorEntry" };

    private string[] AllEntryNames {
        get {
            return new string[] { }.Concat(IntValueEntryNames).Concat(TextValueEntryNames).ToArray();
        }
    }
    private void UpdatePositionFromDrawing() {
        Left = (int)roiRectangle.Left;
        Top = (int)roiRectangle.Top;
        Height = roiRectangle.Height;
        Width = roiRectangle.Width;
        HeightEntry = height.ToString();
        WidthEntry = width.ToString();
        TopEntry = top.ToString();
        LeftEntry = left.ToString();

        Thickness = (int)roiRectangle.LineWidth;
        ThicknessEntry = thickness.ToString();

        //revalidate entries
        foreach (var item in AllEntryNames) {
            OnPropertyChanged(item);
        }

    }

    ~RoiInfoControlViewModel() {
        this.roiRectangle.DrawingChanged -= GraphicsRoiRectangle_DrawingChanged;
    }
}