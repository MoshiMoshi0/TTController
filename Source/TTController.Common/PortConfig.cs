using System.ComponentModel;

namespace TTController.Common
{
    public class PortConfig
    {
        [DefaultValue("Unknown")] public string Name { get; internal set; } = "Unknown";
        [DefaultValue("Default")] public string DeviceType { get; internal set; } = "Default";
        [DefaultValue(LedCountHandling.Trim)] public LedCountHandling LedCountHandling { get; internal set; } = LedCountHandling.Trim;
        [DefaultValue(null)] public int[] LedRotation { get; internal set; } = null;
        [DefaultValue(null)] public bool[] LedReverse { get; internal set; } = null;
        [DefaultValue(false)] public bool IgnoreColorCache { get; internal set; } = false;
        [DefaultValue(false)] public bool IgnoreSpeedCache { get; internal set; } = false;

        public static readonly PortConfig Default = new PortConfig();
    }
}
