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
}
