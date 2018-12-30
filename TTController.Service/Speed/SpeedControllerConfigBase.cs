using TTController.Service.Trigger;

namespace TTController.Service.Speed
{
    public abstract class SpeedControllerConfigBase
    {
        public ITriggerBase Trigger { get; set; }
    }
}
