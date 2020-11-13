using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.TrimLedColorModifier
{
    public class TrimLedColorModifierConfig : ModifierConfigBase
    {
        [DefaultValue(false)] public bool WrapRemainder { get; internal set; } = false;
    }

    public class TrimLedColorModifier : LedColorModifierBase<TrimLedColorModifierConfig>
    {
        public TrimLedColorModifier(TrimLedColorModifierConfig config) : base(config) { }

        public override void Apply(ref List<LedColor> colors) => throw new NotImplementedException();
        public override void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache)
        {
            var ledCount = cache.GetDeviceConfig(port).LedCount;
            if (ledCount <= colors.Count)
                return;

            if (Config.WrapRemainder)
            {
                var wrapCount = (int)Math.Floor(colors.Count / (double)ledCount);
                var startOffset = (wrapCount - 1) * ledCount;
                var remainder = colors.Count - wrapCount * ledCount;

                colors = colors.Skip(colors.Count - remainder)
                    .Concat(colors.Skip(startOffset + remainder).Take(ledCount - remainder))
                    .ToList();
            }
            else
            {
                colors.RemoveRange(ledCount, colors.Count - ledCount);
            }
        }
    }
}
