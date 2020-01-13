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
        [DefaultValue(1000)] public int OnTime { get; private set; } = 1000;
        [DefaultValue(1000)] public int OffTime { get; private set; } = 1000;
        [DefaultValue(null)] public LedColor? OnColor { get; private set; } = null;
        [DefaultValue(null)] public LedColor? OffColor { get; private set; } = null;
        [DefaultValue(null)] public List<LedColor> OnColors { get; private set; } = null;
        [DefaultValue(null)] public List<LedColor> OffColors { get; private set; } = null;
    }

    public class BlinkEffect : EffectBase<BlinkEffectConfig>
    {
        private int _ticks;
        private bool _state;

        public BlinkEffect(BlinkEffectConfig config) : base(config) { }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            List<LedColor> Clone(LedColor? color, int count)
                => color != null ? Enumerable.Repeat(color.Value, count).ToList() : null;

            var current = Environment.TickCount;
            var diff = current - _ticks;

            if ((_state && diff > Config.OnTime) || (!_state && diff > Config.OffTime))
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

                var ledCount = cache.GetDeviceConfig(port).LedCount;
                if (_state)
                    result.Add(port, Clone(Config.OnColor, ledCount) ?? Config.OnColors ?? Clone(new LedColor(0, 0, 0), ledCount));
                else
                    result.Add(port, Clone(Config.OffColor, ledCount) ?? Config.OffColors ?? Clone(new LedColor(0, 0, 0), ledCount));
            }

            return result;
        }
    }
}
