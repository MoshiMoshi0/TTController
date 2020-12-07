using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public interface IPortModifier : IModifierBase
    {
        void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache);
    }
}
