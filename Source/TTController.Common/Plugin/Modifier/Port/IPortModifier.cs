using System.Collections.Generic;

namespace TTController.Common.Plugin
{
    public interface IPortModifier : IModifierBase
    {
        void Apply(ref List<LedColor> colors, PortIdentifier port, ICacheProvider cache);
    }
}
