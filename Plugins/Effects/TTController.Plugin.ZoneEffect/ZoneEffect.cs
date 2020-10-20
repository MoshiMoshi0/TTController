using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.ZoneEffect
{
    public class ZoneEffectConfig : EffectConfigBase
    {
        [DefaultValue(null)] public List<IEffectBase> Effects { get; internal set; } = null;
    }

    public class ZoneEffect : EffectBase<ZoneEffectConfig>
    {
        public ZoneEffect(ZoneEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override void Update(ICacheProvider cache)
        {
            foreach (var effect in Config.Effects)
                effect.Update(cache);
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach(var port in ports)
            {
                var device = cache.GetDeviceConfig(port);
                var colors = new List<LedColor>();
                for(var i = 0; i < device.Zones.Length; i++)
                {
                    if (i >= Config.Effects.Count)
                        continue;

                    var effect = Config.Effects[i];
                    var zoneSize = device.Zones[i];

                    var zoneColors = default(List<LedColor>);
                    try { zoneColors = effect.GenerateColors(zoneSize, cache); }
                    catch (NotImplementedException) { }

                    if (zoneColors == null)
                        zoneColors = Enumerable.Repeat(new LedColor(), zoneSize).ToList();

                    if (zoneColors.Count != zoneSize)
                    {
                        var gradient = new LedColorGradient(colors, zoneSize - 1);

                        colors.Clear();
                        for (var j = 0; j < zoneSize; j++)
                            colors.Add(gradient.GetColor(j));
                    }

                    colors.AddRange(zoneColors);
                }

                result.Add(port, colors);
            }

            return result;
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
            => throw new NotImplementedException();
    }
}
