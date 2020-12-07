using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.ReverseLedColorModifier
{
    public class ReverseLedColorModifierConfig : ModifierConfigBase
    {
        [DefaultValue(null)] public bool? Reverse { get; internal set; } = null;
        [DefaultValue(null)] public bool[] ZoneReverse { get; internal set; } = null;
    }

    public class ReverseLedColorModifier : LedColorModifierBase<ReverseLedColorModifierConfig>
    {
        public ReverseLedColorModifier(ReverseLedColorModifierConfig config) : base(config) { }

        public override void Apply(ref List<LedColor> colors)
        {
            if (Config.Reverse == true)
                colors.Reverse();
        }

        public override void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache)
        {
            if (Config.Reverse != null)
            {
                Apply(ref colors);
            }
            else
            {
                var offset = 0;
                var newColors = new List<LedColor>();
                var zones = cache.GetDeviceConfig(port).Zones;
                for (int i = 0; i < zones.Length; i++)
                {
                    var zoneColors = colors.Skip(offset).Take(zones[i]);
                    if (i < Config.ZoneReverse?.Length && Config.ZoneReverse[i])
                        zoneColors = zoneColors.Reverse();

                    offset += zones[i];
                    newColors.AddRange(zoneColors);
                }

                if (newColors.Count < colors.Count)
                    newColors.AddRange(colors.Skip(offset));

                colors = newColors;
            }
        }
    }
}
