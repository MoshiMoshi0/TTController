using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.ProcessTrigger
{
    public class ProcessTriggerConfig : TriggerConfigBase
    {
        public List<string> Processes { get; internal set; } = new List<string>();
        [DefaultValue(2500)] public int UpdateInterval { get; internal set; } = 2500;
    }

    public class ProcessTrigger : TriggerBase<ProcessTriggerConfig>
    {
        private long _lastTickCount;
        private bool _needsUpdate;

        public ProcessTrigger(ProcessTriggerConfig config) : base(config)
        {
            _lastTickCount = Environment.TickCount;
        }

        public override bool Value(ICacheProvider cache)
        {
            if (Environment.TickCount - _lastTickCount >= Config.UpdateInterval)
            {
                _lastTickCount = Environment.TickCount;

                _needsUpdate = Config.Processes.Any(p => Process.GetProcessesByName(p).Length > 0);
            }

            return _needsUpdate;
        }
    }
}
