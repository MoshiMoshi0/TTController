using System.Collections.Generic;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.FullColorEffect
{
    public class FullColorEffectConfig : EffectConfigBase
    {
        public LedColor Color { get; set; }
    }

    public class FullColorEffect : EffectBase<FullColorEffectConfig>
    {
        public FullColorEffect(FullColorEffectConfig config) : base(config) { }

        public override string EffectType => "ByLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var port in ports)
            {
                var config = cache.GetPortConfig(port);
                if (config == null)
                    continue;

                result.Add(port, Enumerable.Range(0, config.LedCount).Select(x => Config.Color).ToList());
            }

            return result;
        }
    }
}