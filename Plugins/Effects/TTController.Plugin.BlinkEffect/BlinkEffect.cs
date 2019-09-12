using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.BlinkEffect
{
    public class BlinkEffectConfig : EffectConfigBase
    {
        [DefaultValue(1000)] public int On { get; private set; } = 1000;
        [DefaultValue(1000)] public int Off { get; private set; } = 1000;
        public List<LedColor> Colors { get; private set; } = new List<LedColor>();
    }

    public class BlinkEffect : EffectBase<BlinkEffectConfig>
    {
        private int _ticks;
        private bool _state;

        public BlinkEffect(BlinkEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var current = Environment.TickCount;
            var diff = current - _ticks;

            if ((_state && diff > Config.On) || (!_state && diff > Config.Off))
            {
                _ticks = current;
                _state = !_state;
            }

            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var port in ports)
            {
                var config = cache.GetPortConfig(port);
                if (config == null)
                    continue;

                if (_state)
                {
                    result.Add(port, Config.Colors.ToList());
                }
                else
                {
                    var offColor = new LedColor(0, 0, 0);
                    result.Add(port, Enumerable.Range(0, config.DeviceType.GetLedCount()).Select(_ => offColor).ToList());
                }
            }

            return result;
        }
    }
}
