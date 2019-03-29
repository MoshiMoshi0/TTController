using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TTController.Common;
using TTController.Plugin.ProcessTrigger;

namespace TTController.Plugin.ProcessTrigger
{
    public class ProcessTriggerConfig : TriggerConfigBase
    {
        public List<string> Processes { get; private set; } = new List<string>();
    }

    public class ProcessTrigger : TriggerBase<ProcessTriggerConfig>
    {
        private long _ticks;
        private bool _needsUpdate;

        public ProcessTrigger(ProcessTriggerConfig config) : base(config)
        {
            _ticks = Environment.TickCount;
        }

        public override bool Value()
        {
            if (Environment.TickCount - _ticks > 2500)
            {
                _ticks = Environment.TickCount;

                _needsUpdate = Config.Processes.Any(p => Process.GetProcessesByName(p).Length > 0);
            }

            return _needsUpdate;
        }
    }
}
