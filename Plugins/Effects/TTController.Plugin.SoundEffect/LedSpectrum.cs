using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SoundEffect
{
    public class LedSpectrum : SpectrumBase
    {
        private readonly LedColorGradient _colorGradient;

        public LedSpectrum(LedColorGradient colorGradient)
        {
            _colorGradient = colorGradient;
        }

        public IDictionary<PortIdentifier, List<LedColor>> GenerateColors(ColorGenerationMethod generationMethod,
            List<PortIdentifier> ports, ICacheProvider cache, float[] fftBuffer)
        {
            if (generationMethod == ColorGenerationMethod.PerPort)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                List<LedColor> colors = null;
                List<SpectrumPointData> points = null;
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;

                    if (UpdateFrequencyMappingIfNecessary(ledCount) || points == null)
                    {
                        points = CalculateSpectrumPoints(1.0, fftBuffer);
                        colors = GenerateColors(points);
                    }

                    result.Add(port, colors.ToList());
                }

                return result;
            }
            else if (generationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();

                UpdateFrequencyMappingIfNecessary(totalLedCount);
                var points = CalculateSpectrumPoints(1.0, fftBuffer);
                var colors = GenerateColors(points);
                return EffectUtils.SplitColorsPerPort(colors, ports, cache);
            }

            return null;
        }

        protected bool UpdateFrequencyMappingIfNecessary(int ledCount)
        {
            if(ledCount != SpectrumResolution)
            {
                SpectrumResolution = ledCount;
                UpdateFrequencyMapping();
                return true;
            }

            return false;
        }

        protected List<LedColor> GenerateColors(List<SpectrumPointData> points)
            => points.Select(p => _colorGradient.GetColor(p.Value)).ToList();
    }
}
