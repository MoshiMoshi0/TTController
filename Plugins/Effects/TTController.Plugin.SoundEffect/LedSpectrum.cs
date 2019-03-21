using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.SoundEffect
{
    public class LedSpectrum : SpectrumBase
    {
        public delegate LedColor LedColorGenerator(double fftValue);

        private readonly LedColorGenerator _colorGenerator;

        public LedSpectrum(LedColorGenerator colorGenerator)
        {
            _colorGenerator = colorGenerator;
        }

        public IDictionary<PortIdentifier, List<LedColor>> GenerateColors(ColorGenerationMethod generationMethod,
            List<PortIdentifier> ports, ICacheProvider cache, float[] fftBuffer)
        {
            var points = CalculateSpectrumPoints(1.0, fftBuffer);
            var result = new Dictionary<PortIdentifier, List<LedColor>>();

            if (generationMethod == ColorGenerationMethod.PerPort)
            {
                foreach (var port in ports)
                {
                    var config = cache.GetPortConfig(port);
                    var colors = GenerateColorSpectrum(config.LedCount, points);
                    result.Add(port, colors);
                }

                return result;
            }
            else if (generationMethod == ColorGenerationMethod.SpanPorts)
            {
                var ledCount = ports.Select(p => cache.GetPortConfig(p).LedCount).Sum();
                var colors = GenerateColorSpectrum(ledCount, points);

                var sliceOffset = 0;
                foreach (var port in ports)
                {
                    var config = cache.GetPortConfig(port);
                    result.Add(port, colors.GetRange(sliceOffset, config.LedCount));
                    sliceOffset += config.LedCount;
                }

                return result;
            }

            return null;
        }

        protected List<LedColor> GenerateColorSpectrum(int ledCount, List<SpectrumPointData> points)
        {
            var bucketSize = Math.Round(points.Count / (double)ledCount, 3);

            var colors = new List<LedColor>();
            for (var i = 0; i < ledCount; i++)
            {
                var startIndex = (int)(i * bucketSize);
                var endIndex = (int)((i + 1) * bucketSize);

                var value = 0.0;
                for (var j = startIndex; j < endIndex; j++)
                    value += points[j].Value;
                value /= (endIndex - startIndex);

                colors.Add(_colorGenerator(value));
            }

            return colors;
        }
    }
}
