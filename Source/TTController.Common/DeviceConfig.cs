using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
{
    public class DeviceConfig
    {
        [DefaultValue("Default")] public string Name { get; private set; } = "Default";
        [DefaultValue(12)] public int LedCount { get; private set; } = 12;
        [DefaultValue(new int[] { 12 })] public int[] Zones { get; private set; } = new int[] { 12 };

        public static readonly DeviceConfig Default = new DeviceConfig();
    }
}
