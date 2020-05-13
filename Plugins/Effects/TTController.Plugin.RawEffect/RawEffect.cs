using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RawEffect
{
    public class RawEffectConfig : EffectConfigBase
    {
        [DefaultValue(null)] public string EffectType { get; internal set; } = null;
        public LedColorProvider Color { get; internal set; } = new LedColorProvider();
    }

    public class RawEffect : EffectBase<RawEffectConfig>
    {
        public override string EffectType => Config.EffectType;

        public RawEffect(RawEffectConfig config) : base(config) { }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, p => Config.Color.Get(cache.GetDeviceConfig(p).LedCount).ToList());
        }
    }
}
