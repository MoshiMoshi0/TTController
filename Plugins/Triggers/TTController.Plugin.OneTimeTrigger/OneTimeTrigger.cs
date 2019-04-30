using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.OneTimeTrigger
{
    public class OneTimeTriggerConfig : TriggerConfigBase { }

    public class OneTimeTrigger : TriggerBase<OneTimeTriggerConfig>
    {
        private bool _triggered;

        public OneTimeTrigger(OneTimeTriggerConfig config) : base(config)
        {
            _triggered = false;
        }

        public override bool Value(ICacheProvider cache)
        {
            if (_triggered)
                return false;

            _triggered = true;
            return true;
        }
    }
}
