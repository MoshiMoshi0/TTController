using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public interface IEffectModifier : IModifierBase
    {
        void Apply(ref List<LedColor> colors, ICacheProvider cache);
    }
}
