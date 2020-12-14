using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.TrimPortModifier
{
    public class TrimPortModifierConfig : ModifierConfigBase
    {
        [DefaultValue(false)] public bool WrapRemainder { get; internal set; } = false;
    }

    public class TrimPortModifier : PortModifierBase<TrimPortModifierConfig>
    {
        public TrimPortModifier(TrimPortModifierConfig config) : base(config) { }

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
