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
    }

    public enum LedCountHandling
    {
        DoNothing,
        Lerp,
        Nearest,
        Wrap,
        Trim,
        Copy
    }

    public enum DeviceType
    {
        Default,
        RiingTrio,
        RiingDuo,
        FloeRiing,
        PurePlus
    }

    public static class EnumExtensions
    {
        public static int GetLedCount(this DeviceType type)
        {
            switch (type)
            {
                default:
                case DeviceType.Default: return 12;
                case DeviceType.RiingDuo: return 18;
                case DeviceType.RiingTrio: return 30;
                case DeviceType.FloeRiing: return 6;
                case DeviceType.PurePlus: return 9;
            }
        }

        public static int[] GetZones(this DeviceType type)
        {
            switch (type)
            {
                default:
                case DeviceType.Default: return new int[] { 12 };
                case DeviceType.RiingDuo: return new int[] { 12, 6 };
                case DeviceType.RiingTrio: return new int[] { 12, 12, 6 };
                case DeviceType.FloeRiing: return new int[] { 6 };
                case DeviceType.PurePlus: return new int[] { 9 };
            }
        }
    }
}
