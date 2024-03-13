
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfIntensityView.Interface;
using SystemDrawing = System.Drawing;

namespace WpfIntensityView.Model;
public class ColorMapping : IColorMapping {

    private string mappingReferenceDirectoryName = "ReferenceImage";
    private Dictionary<string, string> mappingSource = new Dictionary<string, string>();

    private readonly Dictionary<string, BitmapPalette> loadedPalettes = new Dictionary<string, BitmapPalette>();

    public ColorMapping() {
        LoadMappingReference();
    }
    public List<string> GetMappingKeys() {
        return mappingSource.Select(x => x.Key).ToList();
    }
    private void LoadMappingReference() {
        var mappingSourceArray = CheckReferenceSourceBitmapFiles();
        foreach (var item in mappingSourceArray) {
            var image = new SystemDrawing.Bitmap(item.Value);
            var colors = new List<Color>();

            if (image.Width != 256) continue;
            if (image.Height < 1) continue;

            for (var i = 0; i < image.Width; i++) {
                var pixelvalue = image.GetPixel(i, 0);
                colors.Add(Color.FromRgb(pixelvalue.R, pixelvalue.G, pixelvalue.B));
            }

            loadedPalettes.Add(item.Key, new BitmapPalette(colors));
            mappingSource.Add(item.Key, item.Value);
        }
    }

    private KeyValuePair<string, string>[] CheckReferenceSourceBitmapFiles() {
        var mappingSourceFiles = new Dictionary<string, string>();
        var files = Directory.GetFiles(mappingReferenceDirectoryName, "*.bmp");
        if (files.Length > 0) {
            foreach (var file in files) {
                var fileInfo = new FileInfo(file);
                var fileName = fileInfo.Name;
                var fileNameWithoutExtenstion = fileName.Substring(0, fileName.LastIndexOf("."));
                if (!fileNameWithoutExtenstion.EndsWith("Palette")) continue;
                var underscoreIndex = fileName.IndexOf('_');
                if (underscoreIndex == -1) continue;
                var key = fileName.Substring(0, underscoreIndex);
                mappingSourceFiles.Add(key, file);
            }
        }
        return mappingSourceFiles.ToArray();
    }

    public string GetDefaultBitmapPaletteName() {
        if (loadedPalettes.Count <= 0)
            throw new Exception($"Default BitmapPalette Key not available");
        return loadedPalettes.Keys.ElementAt(0);
    }
    public BitmapPalette GetBitmapPalette(string name) {

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(name);

        if (loadedPalettes.ContainsKey(name))
            return loadedPalettes[name];

        throw new KeyNotFoundException($"{name} not loaded yet");
    }

}