using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public abstract class PortModifierBase<T> : ModifierBase<T>, IPortModifier where T : ModifierConfigBase
    {
        protected PortModifierBase(T config) : base(config) { }

        public abstract void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache);
    }
}
