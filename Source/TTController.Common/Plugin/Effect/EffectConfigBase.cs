using System.Collections.Generic;
using System.ComponentModel;

namespace TTController.Common.Plugin
{
    public abstract class EffectConfigBase
    {
        [DefaultValue(null)] public ITriggerBase Trigger { get; internal set; } = null;
        [DefaultValue(ColorGenerationMethod.PerPort)] public ColorGenerationMethod ColorGenerationMethod { get; internal set; } = ColorGenerationMethod.PerPort;
        [DefaultValue(null)] public List<IEffectModifier> Modifiers { get; internal set; } = null;
    }
}
