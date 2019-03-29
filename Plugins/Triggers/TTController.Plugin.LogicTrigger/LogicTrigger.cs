using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;

namespace TTController.Plugin.LogicTrigger
{
    public enum LogicOperation
    {
        And,
        Or
    }

    public class LogicTriggerConfig : TriggerConfigBase
    {
        [DefaultValue(LogicOperation.And)] public LogicOperation Operation { get; private set; } = LogicOperation.And;
        [DefaultValue(false)] public bool Negate { get; private set; } = false;
        public List<ITriggerBase> Triggers { get; private set; } = new List<ITriggerBase>();
    }

    public class LogicTrigger : TriggerBase<LogicTriggerConfig>
    {
        public LogicTrigger(LogicTriggerConfig config) : base(config) { }

        public override bool Value()
        {
            var result = false;
            if(Config.Operation == LogicOperation.And)
                result = Config.Triggers.All(t => t.Value());
            else if(Config.Operation == LogicOperation.Or)
                result = Config.Triggers.Any(t => t.Value());

            if (Config.Negate)
                result = !result;

            return result;
        }
    }
}
