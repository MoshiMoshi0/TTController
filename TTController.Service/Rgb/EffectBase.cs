using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Common.Config;

namespace TTController.Service.Rgb
{
    public enum EffectType
    {
        Flow = 0x00,
        Spectrum = 0x04,
        Ripple = 0x08,
        Blink = 0xc,
        Pulse = 0x10,
        Wave = 0x14,
        ByLed = 0x18,
        Full = 0x19
    }

    public abstract class EffectBase
    {
        protected dynamic Config;
        protected EffectBase(dynamic config) => Config = config;

        public abstract byte EffectByte { get; } 
        public abstract bool NeedsUpdate();
        public abstract IList<IEnumerable<LedColor>> GenerateColors(IEnumerable<PortConfigData> ports);
    }
}
