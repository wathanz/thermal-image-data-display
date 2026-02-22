using DemoAppNet.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using WpfCanvasDrawing;

namespace DemoAppNet;
public partial class RoiInfoControl : UserControl {
    private readonly GraphicsRoiRectangle roiRectangle;
    public event EventHandler OnRoiRemovedClicked;
    private readonly RoiInfoControlViewModel roiInfoCtrlVm;
    public GraphicsRoiRectangle RoiRectangle { get { return this.roiRectangle; } }
    public RoiInfoControl(GraphicsRoiRectangle roiRectangle, Size imageSize) {
        InitializeComponent();

        this.roiRectangle = roiRectangle;
        this.ImageSize = imageSize;

        this.roiInfoCtrlVm = new RoiInfoControlViewModel(this.roiRectangle, imageSize);
        this.DataContext = roiInfoCtrlVm;
    }

    public Size ImageSize { get; }

    private void TxtColorEntry_KeyUp(object sender, KeyEventArgs e) {
        if (e.Key != Key.Enter) return;
        TxtColorEntryUpdate();
    }

    private void TxtColorEntryUpdate() {
        var errorMsg = roiInfoCtrlVm[TxtColorEntry.Tag.ToString()];
        if (!string.IsNullOrWhiteSpace(errorMsg)) return;
        roiRectangle.TrySetColorString(TxtColorEntry.Text);
    }

    private void TxtSizePositionEntry_KeyUp(object sender, KeyEventArgs e) {
        if (e.Key != Key.Enter) return;
        SizePositionEntryDataUpdate(sender as TextBox);
    }

    private void TxtSizePositionEntry_LostFocus(object sender, RoutedEventArgs e) {
        SizePositionEntryDataUpdate(sender as TextBox);
    }

    private void SizePositionEntryDataUpdate(TextBox tb) {
        if (!CheckIfPositionSizeEntryValid(tb, tb.Tag.ToString(), out int value)) return;

        var name = tb.Tag.ToString();
        switch (name) {
            case nameof(roiInfoCtrlVm.HeightEntry):
                roiInfoCtrlVm.Height = value;
                break;
            case nameof(roiInfoCtrlVm.WidthEntry):
                roiInfoCtrlVm.Width = value;
                break;
            case nameof(roiInfoCtrlVm.TopEntry):
                roiInfoCtrlVm.Top = value;
                break;
            case nameof(roiInfoCtrlVm.LeftEntry):
                roiInfoCtrlVm.Left = value;
                break;
            default:
                break;
        }
        roiInfoCtrlVm.UpdateRoiDrawing();
    }

    private bool CheckIfPositionSizeEntryValid(TextBox tb, string name, out int value) {
        value = 0;
        if (tb == null) {
            return false;
        }
        var errorMsg = roiInfoCtrlVm[name];
        var text = tb.Text;
        var valid = string.IsNullOrEmpty(errorMsg);
        if (!valid || !int.TryParse(text, out value)) {
            return false;
        }
        return true;
    }

    private void TxtThicknessEntry_KeyUp(object sender, KeyEventArgs e) {
        if (e.Key != Key.Enter) return;
        UpdateThickness();
    }

    private void UpdateThickness() {
        var text = TxtThicknessEntry.Text;
        if (!double.TryParse(text, out var result)) return;
        if (result <= 0 || result > 10) return;
        roiRectangle.LineWidth = result;
    }
    private void BtnRemove_Click(object sender, RoutedEventArgs e) {
        if (OnRoiRemovedClicked != null)
            OnRoiRemovedClicked(this, e);
    }


    private void TxtTextBottom_KeyUp(object sender, KeyEventArgs e) {
        if (e.Key != Key.Enter) return;
        SetBottomText();
    }

    private void TxtTextTop_KeyUp(object sender, KeyEventArgs e) {
        if (e.Key != Key.Enter) return;
        roiRectangle.TextOnTop = TxtTextTop.Text;
    }

    private void TxtColorEntry_LostFocus(object sender, RoutedEventArgs e) {
        TxtColorEntryUpdate();
    }

    private void TxtThicknessEntry_LostFocus(object sender, RoutedEventArgs e) {
        UpdateThickness();
    }

    private void TxtTextBottom_LostFocus(object sender, RoutedEventArgs e) {
        SetBottomText();
    }

    private void SetBottomText() {
        roiRectangle.TextOnBottom = TxtTextBottom.Text;
    }
}