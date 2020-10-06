using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.FlowEffect
{
    public class FlowEffectConfig : EffectConfigBase
    {
        [DefaultValue(0.05)] public double FillStep { get; internal set; } = 0.05;
        [DefaultValue(30)] public double HueStep { get; internal set; } = 30;
        [DefaultValue(1.0)] public double Saturation { get; internal set; } = 1.0;
        [DefaultValue(1.0)] public double Brightness { get; internal set; } = 1.0;
    }

    public class FlowEffect : EffectBase<FlowEffectConfig>
    {
        private double _currentHue;
        private double _lastHue;
        private double _fill;

        public FlowEffect(FlowEffectConfig config) : base(config)
        {
            _lastHue = 0;
            _currentHue = Config.HueStep;
        }

        public override string EffectType => "PerLed";

        public override void Update(ICacheProvider cache)
        {
            _fill += Config.FillStep;
            if (_fill >= 1)
            {
                _fill = 0;
                _lastHue = _currentHue;
                _currentHue = ((_currentHue + Config.HueStep) % 360 + 360) % 360;
            }
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            if (Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                return EffectUtils.GenerateColorsPerPort(ports, cache, (_, ledCount) => GenerateColors(ledCount, cache));
            }
            else if(Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();
                var colors = GenerateColors(totalLedCount, cache);
                return EffectUtils.SplitColorsPerPort(colors, ports, cache);
            }

            return null;
        }

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            var lastColor = LedColor.FromHsv(_lastHue, Config.Saturation, Config.Brightness);
            var currentColor = LedColor.FromHsv(_currentHue, Config.Saturation, Config.Brightness);

            var fillIndex = (int)Math.Round(count * _fill);
            var colors = new List<LedColor>();
            for (var i = 0; i < count; i++)
                colors.Add((i < fillIndex) ? currentColor : lastColor);

            return colors;
        }
    }
}
