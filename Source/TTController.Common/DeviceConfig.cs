using System.ComponentModel;

namespace TTController.Common
{
    public class DeviceConfig
    {
        [DefaultValue("Default")] public string Name { get; internal set; } = "Default";
        [DefaultValue(12)] public int LedCount { get; internal set; } = 12;
        [DefaultValue(new int[] { 12 })] public int[] Zones { get; internal set; } = new int[] { 12 };

        public static readonly DeviceConfig Default = new DeviceConfig();
    }
}
