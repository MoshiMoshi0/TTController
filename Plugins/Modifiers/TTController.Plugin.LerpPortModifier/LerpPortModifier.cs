using System;
using System.Collections.Generic;
using System.ComponentModel;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.LerpPortModifier
{
    public enum LerpType
    {
        Smooth,
        Nearest
    }

    public class LerpPortModifierConfig : ModifierConfigBase
    {
        [DefaultValue(LerpType.Smooth)] public LerpType LerpType { get; internal set; } = LerpType.Smooth;
    }

    public class LerpPortModifier : PortModifierBase<LerpPortModifierConfig>
    {
        public LerpPortModifier(LerpPortModifierConfig config) : base(config) { }

        public override void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache)
        {
            var ledCount = cache.GetDeviceConfig(port).LedCount;
            if (ledCount == colors.Count)
                return;

            if (Config.LerpType == LerpType.Smooth)
            {
                var newColors = new List<LedColor>();
                var gradient = new LedColorGradient(colors, ledCount - 1);

                for (var i = 0; i < ledCount; i++)
                    newColors.Add(gradient.GetColor(i));

                colors = newColors;
            }
            else if(Config.LerpType == LerpType.Nearest)
            {
                var newColors = new List<LedColor>();
                for (var i = 0; i < ledCount; i++)
                {
                    var idx = (int)Math.Round((i / (ledCount - 1d)) * (colors.Count - 1d));
                    newColors.Add(colors[idx]);
                }

                colors = newColors;
            }
        }
    }
}
