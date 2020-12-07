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

        public override void Update(ICacheProvider cache)
        {
            if (_tick++ >= Config.TickInterval)
            {
                _tick = 0;
                _rotation++;
            }
        }

        protected override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return EffectUtils.GenerateColorsPerPort(ports, cache, (_, ledCount) => GenerateColors(ledCount, cache) );
            }
            else if (Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                return EffectUtils.SplitColorsPerPort(GenerateColors(totalLedCount, cache), ports, cache);
            }

            return null;
        }

        protected override List<LedColor> GenerateColors(int count, ICacheProvider cache)
            => Config.Color.Get(count).RotateRight(_rotation % count).ToList();
    }
}
