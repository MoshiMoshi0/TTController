using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
{
    public class SensorConfig
    {
        [DefaultValue(null)] public float? Offset { get; private set; } = null;
        [DefaultValue(null)] public float? CriticalValue { get; private set; } = null;

        public static readonly SensorConfig Default = new SensorConfig();
    }
}
