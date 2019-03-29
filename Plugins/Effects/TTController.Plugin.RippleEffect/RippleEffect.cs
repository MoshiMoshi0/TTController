using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;

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
        private int _tick;
        private int _rotation;

        public RippleEffect(RippleEffectConfig config) : base(config)
        {
            _tick = Config.TickInterval;
        }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            int Wrap(int a, int b) => (a % b + b) % b;

            if (_tick++ >= Config.TickInterval)
            {
                _tick = 0;
                _rotation = (_rotation + 1);
            }

            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var port in ports)
            {
                var config = cache.GetPortConfig(port);
                if (config == null)
                    continue;
                
                var off = new LedColor(0, 0, 0);
                var colors = Enumerable.Range(0, config.LedCount).Select(x => off).ToList();
                var (hue, saturation, value) = LedColor.ToHsv(Config.Color);
                var length = Config.Length == 0 ? config.LedCount : Config.Length;

                for (var i = 0; i < length; i++)
                {
                    var idx = Wrap(_rotation - i, config.LedCount);
                    colors[idx] = LedColor.FromHsv(hue, saturation, value - (double)i / length);
                }

                result.Add(port, colors);
            }

            return result;
        }
    }
}
