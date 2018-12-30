namespace TTController.Service.Trigger
{
    public class OneTimeTriggerConfig : TriggerConfigBase { }

    public class OneTimeTrigger : TriggerBase<OneTimeTriggerConfig>
    {
        private bool _triggered;

        public OneTimeTrigger(OneTimeTriggerConfig config) : base(config)
        {
            _triggered = false;
        }

        public override bool Value()
        {
            if (_triggered)
                return false;

            _triggered = true;
            return true;
        }
    }
}
