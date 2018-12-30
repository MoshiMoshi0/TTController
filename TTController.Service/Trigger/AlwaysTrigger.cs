namespace TTController.Service.Trigger
{
    public class AlwaysTriggerConfig : TriggerConfigBase { }

    public class AlwaysTrigger : TriggerBase<AlwaysTriggerConfig>
    {
        public AlwaysTrigger(AlwaysTriggerConfig config) : base(config) { }

        public override bool Value() => true;

    }
}
