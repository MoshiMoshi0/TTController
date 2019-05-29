using System.ComponentModel;

namespace TTController.Common
{
    public enum LedCountHandling
    {
        DoNothing,
        Lerp,
        Nearest,
        Wrap,
        Trim,
        Copy
    }

    public class PortConfig
    {
        [DefaultValue("Unknown")] public string Name { get; private set; } = "Unknown";
        [DefaultValue(12)] public int LedCount { get; private set; } = 12;
        [DefaultValue(LedCountHandling.Trim)] public LedCountHandling LedCountHandling { get; private set; } = LedCountHandling.Trim;
        [DefaultValue(0)] public int LedRotation { get; private set; } = 0;
        [DefaultValue(false)] public bool LedReverse { get; private set; } = false;

        public static readonly PortConfig Default = new PortConfig();
    }
}
