using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
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

    public enum EffectSpeed
    {
        Slow = 0x03,
        Normal = 0x02,
        Fast = 0x01,
        Extreme = 0x00
    }

    public enum SensorMixFunction
    {
        Minimum,
        Maximum,
        Average
    };
}
