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
    }
}
