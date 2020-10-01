using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SpectrumEffect
{
    public class SpectrumEffectConfig : EffectConfigBase
    {
        [DefaultValue(1.0f)] public float Saturation { get; internal set; } = 1.0f;
        [DefaultValue(1.0f)] public float Brightness { get; internal set; } = 1.0f;
        [DefaultValue(1.0f)] public float HueStep { get; internal set; } = 1.0f;
    }

    public class SpectrumEffect : EffectBase<SpectrumEffectConfig>
    {
        private float _hue;

        public SpectrumEffect(SpectrumEffectConfig config) : base(config)
        {
            _hue = 0;
        }

        public override string EffectType => "PerLed";

        public override void Update(ICacheProvider cache)
        {
            _hue = (_hue + Config.HueStep) % 360;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
            => ports.ToDictionary(p => p, p => GenerateColors(cache.GetDeviceConfig(p).LedCount, cache));

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            var color = LedColor.FromHsv(_hue, Config.Saturation, Config.Brightness);
            return Enumerable.Repeat(color, count).ToList();
        }
    }
}
