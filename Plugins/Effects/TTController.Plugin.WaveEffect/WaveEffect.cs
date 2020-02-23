using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.WaveEffect
{
    public class WaveEffectConfig : EffectConfigBase
    {
        public List<LedColor> Colors { get; internal set; } = new List<LedColor>();
        [DefaultValue(3)] public int TickInterval { get; internal set; } = 3;
    }

    public class WaveEffect : EffectBase<WaveEffectConfig>
    {
        private int _tick;
        private readonly LinkedList<LedColor> _colors;

        public WaveEffect(WaveEffectConfig config) : base(config)
        {
            _tick = Config.TickInterval;
            _colors = new LinkedList<LedColor>(Config.Colors);
        }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (_tick++ >= Config.TickInterval)
            {
                _tick = 0;
                _colors.AddFirst(_colors.Last.Value);
                _colors.RemoveLast();
            }

            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return ports.ToDictionary(p => p, _ => _colors.ToList());
            }
            else if(Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                var offset = 0;
                foreach(var port in ports)
                {
                    var config = cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    result.Add(port, _colors.Skip(offset).Take(ledCount).ToList());
                    offset += ledCount;
                }

                return result;
            }

            return null;
        }
    }
}
