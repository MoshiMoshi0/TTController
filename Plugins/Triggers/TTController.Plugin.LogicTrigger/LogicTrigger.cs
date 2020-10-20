using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.LogicTrigger
{
    public enum LogicOperation
    {
        And,
        Or
    }

    public class LogicTriggerConfig : TriggerConfigBase
    {
        [DefaultValue(LogicOperation.And)] public LogicOperation Operation { get; internal set; } = LogicOperation.And;
        [DefaultValue(false)] public bool Negate { get; internal set; } = false;
        public List<ITriggerBase> Triggers { get; internal set; } = new List<ITriggerBase>();
    }

    public class LogicTrigger : TriggerBase<LogicTriggerConfig>
    {
        public LogicTrigger(LogicTriggerConfig config) : base(config) { }

        public override bool Value(ICacheProvider cache)
        {
            var result = false;
            if(Config.Operation == LogicOperation.And)
                result = Config.Triggers.All(t => t.Value(cache));
            else if(Config.Operation == LogicOperation.Or)
                result = Config.Triggers.Any(t => t.Value(cache));

            if (Config.Negate)
                result = !result;

            return result;
        }
    }
}
