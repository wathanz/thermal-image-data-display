using System.Windows.Media.Imaging;

namespace WpfIntensityView.Interface;
public interface IIntensityDataMapBitmap {
    WriteableBitmap WriteableSource { get; }
    void Update(int width, int height, byte[] data, string colorMappingName, out bool newImgeSize);
}