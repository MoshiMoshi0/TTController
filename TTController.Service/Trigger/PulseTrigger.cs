using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Service.Trigger
{
    public class PulseTriggerConfig : TriggerConfigBase
    {
        public int On { get; set; } = 1000;
        public int Off { get; set; } = 1000;
    }

    public class PulseTrigger : TriggerBase<PulseTriggerConfig>
    {
        private int _ticks;
        private bool _state;

        public PulseTrigger(PulseTriggerConfig config) : base(config)
        {
            _ticks = Environment.TickCount;
        }

        public override bool Value()
        {
            var current = Environment.TickCount;
            var diff = current - _ticks;

            if ((_state && diff > Config.On) || (!_state && diff > Config.Off))
            {
                _ticks = current;
                _state = !_state;
            }

            return _state;
        }
    }
}
