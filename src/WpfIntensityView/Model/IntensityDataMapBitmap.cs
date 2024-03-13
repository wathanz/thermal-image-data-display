using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfIntensityView.Interface;

namespace WpfIntensityView.Model;
public class IntensityDataMapBitmap : IIntensityDataMapBitmap {

    WriteableBitmap writeableSourceClone;
    WriteableBitmap writeableSource;
    public WriteableBitmap WriteableSource {
        get {
            lock (syncObject) {
                return writeableSourceClone;
            }
        }
    }
    private object syncObject = new();
    private string colorMappingName = "";
    private readonly IColorMapping mapping;

    public IntensityDataMapBitmap(IColorMapping mapping) {
        this.mapping = mapping;
        Update(480, 360, new byte[480 * 360], mapping.GetDefaultBitmapPaletteName(), out _);
    }
    public void Update(int width, int height, byte[] data, string colorMappingName, out bool newImgeSize) {
        lock (syncObject) {
            int stride = (width * 8 + 7) / 8;
            if (IsCreateNew(width, height, colorMappingName, out newImgeSize)) {
                this.colorMappingName = colorMappingName;
                var colorMapping = mapping.GetBitmapPalette(colorMappingName);
                var source = BitmapSource.Create(width, height, 96, 96, PixelFormats.Indexed8, colorMapping, data, stride);
                writeableSource = new WriteableBitmap(source);
                CloneWritableSource();
                return;
            }
            writeableSource.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), data, stride, 0);
            CloneWritableSource();
        }
    }

    private void CloneWritableSource() {
        writeableSourceClone = writeableSource.Clone();
    }

    private bool IsCreateNew(int width, int height, string colorMappingName, out bool newImgeSize) {

        newImgeSize = writeableSource == null ||
            width != writeableSource.PixelWidth ||
            height != writeableSource.PixelHeight;

        if (newImgeSize)
            return true;

        if (colorMappingName != this.colorMappingName)
            return true;

        return false;
    }
}