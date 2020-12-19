using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public static class EffectUtils
    {
        public static Dictionary<PortIdentifier, List<LedColor>> GenerateColorsPerPort(List<PortIdentifier> ports, ICacheProvider cache, Func<PortIdentifier, int, List<LedColor>> generator)
            => ports.ToDictionary(p => p, p => generator(p, cache.GetDeviceConfig(p).LedCount));

        public static Dictionary<PortIdentifier, List<LedColor>> SplitColorsPerPort(List<LedColor> colors, List<PortIdentifier> ports, ICacheProvider cache)
        {
            var result = new Dictionary<PortIdentifier, List<LedColor>>();

            var offset = 0;
            foreach (var port in ports)
            {
                var ledCount = cache.GetDeviceConfig(port).LedCount;
                result.Add(port, colors.GetRange(offset, ledCount));
                offset += ledCount;
            }

            return result;
        }

        public static Dictionary<PortIdentifier, List<LedColor>> SplitMirroredColorsPerPort(List<LedColor> colors, List<PortIdentifier> ports, ICacheProvider cache)
        {
            var result = new Dictionary<PortIdentifier, List<LedColor>>();

            var offset = 0;
            foreach (var port in ports)
            {
                var ledCount = cache.GetDeviceConfig(port).LedCount;
                var halfLedCount = (int)Math.Ceiling(ledCount / 2.0f);
                var remainder = ledCount - halfLedCount;

                var portColors = colors.GetRange(offset, halfLedCount);
                portColors.AddRange(colors.GetRange(colors.Count - remainder - offset, remainder));

                result.Add(port, portColors);
                offset += halfLedCount;
            }

            return result;
        }

        public static List<LedColor> GenerateMirroredColors(int count, Func<int, int, LedColor> generator)
        {
            var colors = Enumerable.Repeat(new LedColor(), count).ToList();

            var halfCount = (int)Math.Ceiling(count / 2.0f);
            for (var i = 0; i < halfCount; i++)
            {
                var color = generator(i, halfCount);

                colors[i] = color;
                colors[count - i - 1] = color;
            }

            return colors;
        }
    }
}
