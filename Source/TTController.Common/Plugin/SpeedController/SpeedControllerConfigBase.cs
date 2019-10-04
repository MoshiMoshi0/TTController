using System.ComponentModel;

namespace TTController.Common.Plugin
{
    public abstract class SpeedControllerConfigBase
    {
        [DefaultValue(null)] public ITriggerBase Trigger { get; private set; } = null;
    }
}
