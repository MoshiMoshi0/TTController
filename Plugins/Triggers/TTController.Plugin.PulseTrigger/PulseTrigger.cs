using System;
using System.ComponentModel;
using TTController.Common;

namespace TTController.Plugin.PulseTrigger
{
    public class PulseTriggerConfig : TriggerConfigBase
    {
        [DefaultValue(1000)] public int On { get; private set; } = 1000;
        [DefaultValue(1000)] public int Off { get; private set; } = 1000;
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
