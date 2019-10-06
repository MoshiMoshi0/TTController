using System.ComponentModel;

namespace TTController.Common
{
    public class PortConfig
    {
        [DefaultValue("Unknown")] public string Name { get; private set; } = "Unknown";
        [DefaultValue("Default")] public string DeviceType { get; private set; } = "Default";
        [DefaultValue(LedCountHandling.Trim)] public LedCountHandling LedCountHandling { get; private set; } = LedCountHandling.Trim;
        [DefaultValue(null)] public int[] LedRotation { get; private set; } = null;
        [DefaultValue(null)] public bool[] LedReverse { get; private set; } = null;

        public static readonly PortConfig Default = new PortConfig();
    }
}
