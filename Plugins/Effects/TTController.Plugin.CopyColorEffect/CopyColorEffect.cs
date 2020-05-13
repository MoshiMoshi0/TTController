using System;
using System.Collections.Generic;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.CopyColorEffect
{
    public class CopyColorEffectConfig : EffectConfigBase
    {
        public PortIdentifier Target { get; internal set; }
    }

    public class CopyColorEffect : EffectBase<CopyColorEffectConfig>
    {
        public CopyColorEffect(CopyColorEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var colors = cache.GetPortColors(Config.Target);
            if (colors == null)
                return null;

            return ports.ToDictionary(p => p, _ => colors);
        }
    }
}
