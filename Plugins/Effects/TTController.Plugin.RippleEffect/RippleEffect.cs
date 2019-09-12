using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RippleEffect
{
    public class RippleEffectConfig : EffectConfigBase
    {
        public LedColor Color { get; private set; } = new LedColor(0, 0, 0);
        [DefaultValue(5)] public int Length { get; private set; } = 5;
        [DefaultValue(3)] public int TickInterval { get; private set; } = 3;
    }

    public class RippleEffect : EffectBase<RippleEffectConfig>
    {
        private readonly LedColor[] _rippleColors;

        private int _tick;
        private int _rotation;

        public RippleEffect(RippleEffectConfig config) : base(config)
        {
            _tick = Config.TickInterval;
            _rippleColors = new LedColor[Config.Length];

            var (hue, saturation, value) = LedColor.ToHsv(Config.Color);
            for (var i = 0; i < Config.Length; i++)
                _rippleColors[i] = LedColor.FromHsv(hue, saturation, value * (Config.Length - i - 1) / (Config.Length - 1));
        }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            int Wrap(int a, int b) => (a % b + b) % b;

            if (_tick++ >= Config.TickInterval)
            {
                _tick = 0;
                _rotation++;
            }

            var result = new Dictionary<PortIdentifier, List<LedColor>>();

            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                foreach (var port in ports)
                {
                    var config = cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    var off = new LedColor(0, 0, 0);
                    var colors = Enumerable.Range(0, config.DeviceType.GetLedCount()).Select(_ => off).ToList();
                    for (var i = 0; i < Config.Length; i++)
                    {
                        var idx = Wrap(_rotation - i, config.DeviceType.GetLedCount());
                        colors[idx] = _rippleColors[i];
                    }

                    result.Add(port, colors);
                }
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLength = ports.Select(p => cache.GetPortConfig(p)).Sum(c => c.DeviceType.GetLedCount());

                var off = new LedColor(0, 0, 0);
                var colors = Enumerable.Range(0, totalLength).Select(_ => off).ToList();
                for (var i = 0; i < Config.Length; i++)
                {
                    var idx = Wrap(_rotation - i, totalLength);
                    colors[idx] = _rippleColors[i];
                }

                var offset = 0;
                foreach (var port in ports)
                {
                    var config = cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    result.Add(port, colors.Skip(offset).Take(config.DeviceType.GetLedCount()).ToList());
                    offset += config.DeviceType.GetLedCount();
                }
            }

            return result;
        }
    }
}
