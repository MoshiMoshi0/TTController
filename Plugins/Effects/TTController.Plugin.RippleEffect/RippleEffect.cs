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
        public LedColorProvider RippleColor { get; internal set; } = new LedColorProvider();
        public LedColorProvider BackgroundColor { get; internal set; } = new LedColorProvider();
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
                return EffectUtils.GenerateColorsPerPort(ports, cache, (port, ledCount) => GenerateColors(ledCount));
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
                    var halfLedCount = ledCount / 2;

                    var topColors = colors.GetRange(offset, halfLedCount);
                    var bottomColors = colors.GetRange(colors.Count - offset - halfLedCount, halfLedCount);
                    result.Add(port, topColors.Concat(bottomColors).ToList());

                    offset += halfLedCount;
                }


                return result;
            }

            return null;
        }

        private List<LedColor> GenerateColors(int size)
        {
            int Wrap(int a, int b) => (a % b + b) % b;

            var colors = Config.BackgroundColor.Get(size).ToList();
            for (var i = 0; i < Config.Length; i++)
            {
                var idx = Wrap(_rotation - i, size);
                colors[idx] = Config.RippleColor.Get(i, Config.Length);
            }

            return colors;
        }
    }
}
