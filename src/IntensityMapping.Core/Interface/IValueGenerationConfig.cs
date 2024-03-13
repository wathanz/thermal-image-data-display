using System;

namespace IntensityMapping.Core.Interface;
public interface IValueGenerationConfig {
    double DataMax { get; set; }
    double DataMin { get; set; }
    int Interval { get; set; }
}