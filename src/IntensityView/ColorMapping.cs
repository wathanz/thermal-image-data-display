
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SystemDrawing = System.Drawing;

namespace IntensityView {
    public static class ColorMapping {

        public static string Rainbow = "Rainbow";
        public static string RainbowFlip = "RainbowFlip";
        public static string Ironbow = "Ironbow";
        public static string Whitehot = "Whitehot";
        public static string Blackhot = "Blackhot";

        private static readonly Dictionary<string, string> mappingSource =
            new Dictionary<string, string>() {
                { Rainbow,"ReferenceImage/Rainbow_Palette.bmp" },
                { RainbowFlip,"ReferenceImage/Rainbow_Palette_flip.bmp" },
                { Ironbow,"ReferenceImage/Ironbow_Palette.bmp" },
                { Whitehot,"ReferenceImage/Whitehot_Palette.bmp" },
                { Blackhot,"ReferenceImage/Blackhot_Palette.bmp" }
            };

        private static bool hasLoaded = false;
        private static readonly Dictionary<string, BitmapPalette> loadedPalettes = new Dictionary<string, BitmapPalette>();

        public static List<string> GetMappingKeys() {
            LoadIfNotLoadedYet();
            return mappingSource.Select(x => x.Key).ToList();
        }
        public static void LoadMappingReference() {
            foreach (var item in mappingSource) {
                var image = new SystemDrawing.Bitmap(item.Value);
                var colors = new List<Color>();
                if (image.Width != 256) continue;
                for (var i = 0; i < image.Width; i++) {
                    var pixelvalue = image.GetPixel(i, 0);
                    colors.Add(Color.FromRgb(pixelvalue.R, pixelvalue.G, pixelvalue.B));
                }
                loadedPalettes.Add(item.Key, new BitmapPalette(colors));
            }
            hasLoaded = true;
        }

        private static void LoadIfNotLoadedYet() {
            if (hasLoaded) return;
            LoadMappingReference();
        }

        public static BitmapPalette GetBitmapPalette(string name) {
            LoadIfNotLoadedYet();
            if (loadedPalettes.ContainsKey(name))
                return loadedPalettes[name];
            throw new KeyNotFoundException($"{name} not loaded yet");
        }

    }
}