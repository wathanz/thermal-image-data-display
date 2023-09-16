using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IntensityView {
    public class IntensityBitmapView {

        WriteableBitmap writeableSource;
        public WriteableBitmap WriteableSource {
            get {
                lock (syncObject) {
                    return writeableSource;
                }
            }
        }
        private object syncObject = new object();
        private string colorMappingName = "";
        public IntensityBitmapView() {

        }
        public void Update(int width, int height, byte[] data, string colorMappingName, out bool newImgeSize) {
            lock (syncObject) {
                int stride = (width * 8 + 7) / 8;
                if (IsCreateNew(width, height, colorMappingName, out newImgeSize)) {
                    this.colorMappingName = colorMappingName;
                    var colorMapping = ColorMapping.GetBitmapPalette(colorMappingName);
                    var source = BitmapSource.Create(width, height, 96, 96, PixelFormats.Indexed8, colorMapping, data, stride);
                    writeableSource = new WriteableBitmap(source);
                    return;
                }
                writeableSource.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), data, stride, 0);
            }
        }

        private bool IsCreateNew(int width, int height, string colorMappingName, out bool newImgeSize) {

            newImgeSize = writeableSource == null ||
                width != writeableSource.Width ||
                height != writeableSource.Height;

            if (newImgeSize)
                return true;

            if (colorMappingName != this.colorMappingName)
                return true;

            return false;
        }
    }
}