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
        [DefaultValue(0.05)] public double FillStep { get; private set; } = 0.05;
        [DefaultValue(30)] public double HueStep { get; private set; } = 30;
        [DefaultValue(1.0)] public double Saturation { get; private set; } = 1.0;
        [DefaultValue(1.0)] public double Brightness { get; private set; } = 1.0;
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

        public override string EffectType => "ByLed";

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
                    var config = cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    var colors = new List<LedColor>();
                    for (var i = 0; i < config.LedCount; i++)
                    {
                        if (i < (int)Math.Round(config.LedCount * _fill))
                            colors.Add(currentColor);
                        else
                            colors.Add(lastColor);
                    }

                    result.Add(port, colors);
                }
            }
            else if(Config.ColorGenerationMethod == ColorGenerationMethod.SpanPorts)
            {
                var totalLength = ports.Select(p => cache.GetPortConfig(p)).Sum(c => c?.LedCount ?? 0);

                var colors = new List<LedColor>();
                for (var i = 0; i < totalLength; i++)
                {
                    if (i < (int)Math.Round(totalLength * _fill))
                        colors.Add(currentColor);
                    else
                        colors.Add(lastColor);
                }

                var offset = 0;
                foreach (var port in ports)
                {
                    var config = cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    result.Add(port, colors.Skip(offset).Take(config.LedCount).ToList());
                    offset += config.LedCount;
                }
            }

            return result;
        }
    }
}
