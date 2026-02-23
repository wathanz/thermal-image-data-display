using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace WpfIntensityView.Interface;
public interface IColorMapping {
    BitmapPalette GetBitmapPalette(string name);
    string GetDefaultBitmapPaletteName();
    List<string> GetMappingKeys();
}