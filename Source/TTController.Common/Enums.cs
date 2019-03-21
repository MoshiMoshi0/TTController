using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
{
    public enum ColorGenerationMethod
    {
        PerPort,
        SpanPorts
    }

    public enum SensorMixFunction
    {
        Minimum,
        Maximum,
        Average
    };
}
