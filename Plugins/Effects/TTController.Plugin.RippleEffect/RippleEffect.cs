using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RippleEffect
{
    public class RippleEffectConfig : EffectConfigBase
    {
        [DefaultValue(5)] public int Length { get; internal set; } = 5;
        [DefaultValue(3)] public int TickInterval { get; internal set; } = 3;
        public LedColorProvider Color { get; internal set; } = new LedColorProvider();
    }

    public class RippleEffect : EffectBase<RippleEffectConfig>
    {
        private int _tick;
        private int _rotation;

        public RippleEffect(RippleEffectConfig config) : base(config)
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
                    result.Add(port, GenerateColors(ledCount));
                }

                return result;
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var result = new Dictionary<PortIdentifier, List<LedColor>>();
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = GenerateColors(totalLedCount);

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

        private List<LedColor> GenerateColors(int size)
        {
            int Wrap(int a, int b) => (a % b + b) % b;

            var off = new LedColor(0, 0, 0);

            var colors = Enumerable.Range(0, size).Select(_ => off).ToList();
            for (var i = 0; i < Config.Length; i++)
            {
                var idx = Wrap(_rotation - i, size);
                var color = Config.Color.Get(idx, size);
                colors[idx] = color.SetValue(color.GetValue() * (Config.Length - i - 1) / (Config.Length - 1));
            }

            return colors;
        }
    }
}
