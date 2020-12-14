using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class EffectModifierBase<T> : ModifierBase<T>, IEffectModifier where T : ModifierConfigBase
    {
        protected EffectModifierBase(T config) : base(config) { }

        public abstract void Apply(ref List<LedColor> colors, ICacheProvider cache);
    }
}
