using System.ComponentModel;

namespace TTController.Common
{
    public class PortConfig
    {
        [DefaultValue("Unknown")] public string Name { get; private set; } = "Unknown";
        [DefaultValue(DeviceType.Default)] public DeviceType DeviceType { get; private set; } = DeviceType.Default;
        [DefaultValue(LedCountHandling.Trim)] public LedCountHandling LedCountHandling { get; private set; } = LedCountHandling.Trim;
        [DefaultValue(0)] public int LedRotation { get; private set; } = 0;
        [DefaultValue(false)] public bool LedReverse { get; private set; } = false;

        public static readonly PortConfig Default = new PortConfig();
    }
}
