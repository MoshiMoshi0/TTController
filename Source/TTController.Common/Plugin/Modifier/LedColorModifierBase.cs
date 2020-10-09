using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class LedColorModifierBase<T> : ModifierBase<T>, ILedColorModifierBase where T : ModifierConfigBase
    {
        protected LedColorModifierBase(T config) : base(config) { }

        public abstract void Apply(ref List<LedColor> colors);
        public abstract void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache);
    }
}
