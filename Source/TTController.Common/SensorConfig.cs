using System.ComponentModel;

namespace TTController.Common
{
    public class SensorConfig
    {
        [DefaultValue(null)] public float? Offset { get; private set; } = null;
        [DefaultValue(null)] public float? CriticalValue { get; private set; } = null;

        public static readonly SensorConfig Default = new SensorConfig();
    }
}
