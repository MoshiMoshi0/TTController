using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Service.Rgb;

namespace TTController.Service.Trigger
{
    public class ProcessTriggerConfig : TriggerConfigBase
    {
        public string[] Processes { get; set; }
    }

    public class ProcessTrigger : ConfigurableTriggerBase<ProcessTriggerConfig>
    {
        private long _ticks;
        private bool _needsUpdate;

        public ProcessTrigger(dynamic rawConfig) : base((object)rawConfig)
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
