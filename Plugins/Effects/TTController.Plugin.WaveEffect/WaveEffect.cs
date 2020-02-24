using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.WaveEffect
{
    public class WaveEffectConfig : EffectConfigBase
    {
        [DefaultValue(3)] public int TickInterval { get; internal set; } = 3;
        public LedColorProvider Color { get; internal set; } = new LedColorProvider();
    }

    public class WaveEffect : EffectBase<WaveEffectConfig>
    {
        private int _tick;
        private int _rotation;

        public WaveEffect(WaveEffectConfig config) : base(config)
        {
            _tick = Config.TickInterval;
        }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (_tick++ >= Config.TickInterval)
            {
                _tick = 0;
                _rotation++;
            }

            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();

                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    var colors = Config.Color.Get(ledCount).RotateRight(_rotation % cache.GetDeviceConfig(port).LedCount).ToList();
                    result.Add(port, colors);
                }

                return result;
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = Config.Color.Get(totalLedCount).RotateRight(_rotation % totalLedCount);

                var offset = 0;
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    result.Add(port, colors.Skip(offset).Take(ledCount).ToList());
                    offset += ledCount;
                }

                return result;
            }

            return null;
        }
    }
}
