using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RotatePortModifier
{
    public class RotatePortModifierConfig : ModifierConfigBase
    {
        [DefaultValue(null)] public int? Rotation { get; internal set; } = null;
        [DefaultValue(null)] public int[] ZoneRotation { get; internal set; } = null;
    }

    public class RotatePortModifier : PortModifierBase<RotatePortModifierConfig>
    {
        public RotatePortModifier(RotatePortModifierConfig config) : base(config) { }

        public override void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache)
        {
            if(Config.Rotation != null)
            {
                var rotate = Config.Rotation.Value;
                if (rotate > 0)
                    colors = colors.RotateRight(rotate).ToList();
                else if (rotate < 0)
                    colors = colors.RotateLeft(rotate).ToList();
            }
            else
            {
                var offset = 0;
                var newColors = new List<LedColor>();
                var zones = cache.GetDeviceConfig(port).Zones;
                for (int i = 0; i < zones.Length; i++)
                {
                    var zoneColors = colors.Skip(offset).Take(zones[i]);

                    if (i < Config.ZoneRotation?.Length)
                    {
                        var rotate = Config.ZoneRotation[i];
                        if (rotate > 0)
                            zoneColors = zoneColors.RotateLeft(rotate);
                        else if (rotate < 0)
                            zoneColors = zoneColors.RotateRight(rotate);
                    }

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
