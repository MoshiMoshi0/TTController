using TTController.Common.Trigger;

namespace TTController.Plugin.AlwaysTrigger
{
    public class AlwaysTriggerConfig : TriggerConfigBase { }

    public class AlwaysTrigger : TriggerBase<AlwaysTriggerConfig>
    {
        public AlwaysTrigger(AlwaysTriggerConfig config) : base(config) { }

        public override bool Value() => true;

    }
}
