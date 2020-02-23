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

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _fill += Config.FillStep;
            if (_fill >= 1)
            {
                _fill = 0;
                _lastHue = _currentHue;
                _currentHue = ((_currentHue + Config.HueStep) % 360 + 360) % 360;
            }

            var lastColor = LedColor.FromHsv(_lastHue, Config.Saturation, Config.Brightness);
            var currentColor = LedColor.FromHsv(_currentHue, Config.Saturation, Config.Brightness);

            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            if(Config.ColorGenerationMethod == ColorGenerationMethod.PerPort)
            {
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    var colors = new List<LedColor>();
                    for (var i = 0; i < ledCount; i++)
                    {
                        if (i < (int)Math.Round(ledCount * _fill))
                            colors.Add(currentColor);
                        else
                            colors.Add(lastColor);
                    }

                    result.Add(port, colors);
                }
            }
            else if(Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLedCount = ports.Select(p => cache.GetDeviceConfig(p).LedCount).Sum();

                var colors = new List<LedColor>();
                for (var i = 0; i < totalLedCount; i++)
                {
                    if (i < (int)Math.Round(totalLedCount * _fill))
                        colors.Add(currentColor);
                    else
                        colors.Add(lastColor);
                }

                var offset = 0;
                foreach (var port in ports)
                {
                    var ledCount = cache.GetDeviceConfig(port).LedCount;
                    result.Add(port, colors.Skip(offset).Take(ledCount).ToList());
                    offset += ledCount;
                }
            }

            return result;
        }
    }
}
