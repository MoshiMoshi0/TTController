using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public interface IEffectModifier : IModifierBase
    {
        void Apply(ref List<LedColor> colors, ICacheProvider cache);
    }
}
