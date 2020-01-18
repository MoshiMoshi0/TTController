using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.PingPongEffect
{
    public class PingPongEffectConfig : EffectConfigBase
    {
        [DefaultValue(0.01f)] public float Step { get; private set; } = 0.01f;
        [DefaultValue(0.2f)] public float Height { get; private set; } = 0.2f;
        [DefaultValue(0.5f)] public float Width { get; private set; } = 0.5f;
        public LedColorGradient ColorGradient { get; private set; } = new LedColorGradient();
        [DefaultValue(true)] public bool EnableSmoothing { get; private set; } = true;
    }

    public class PingPongEffect : EffectBase<PingPongEffectConfig>
    {
        private float _t;
        private int _direction;

        public PingPongEffect(PingPongEffectConfig config) : base(config) 
        {
            _t = 0;
            _direction = 1;
        }

        public override string EffectType => "PerLed";

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _t += Config.Step * _direction;
            if (_t <= 0 || _t >= 1)
                _direction = 1 - _direction;

            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            for (var i = 0; i < ports.Count; i++)
            {
                var port = ports[i];
                var config = cache.GetDeviceConfig(port);
                var ledCount = config.LedCount;

                var globalStart = i / (double)ports.Count;
                var globalEnd = (i + 1) / (double)ports.Count;

                var tBottom = _t - Config.Height / 2;
                var tTop = _t + Config.Height / 2;
                if ((tBottom < globalStart && tTop < globalStart) || (tBottom > globalEnd && tTop > globalEnd))
                {
                    result.Add(port, Enumerable.Range(0, ledCount).Select(_ => new LedColor()).ToList());
                }
                else if ((tBottom >= globalStart && tBottom <= globalEnd)
                    || (tTop >= globalStart && tTop <= globalEnd)
                    || (tBottom < globalStart && tTop > globalEnd))
                {
                    var colors = new List<LedColor>();
                    switch (config.Name)
                    {
                        case "RiingTrio":
                            colors.AddRange(GenerateColors(12, globalStart, globalEnd));
                            colors.AddRange(colors.ToList());
                            colors.AddRange(GenerateColors(6, globalStart, globalEnd, radius: 0.33, oddDivide: false));
                            break;
                        case "RiingDuo":
                            colors.AddRange(GenerateColors(12, globalStart, globalEnd));
                            colors.AddRange(GenerateColors(6, globalStart, globalEnd, radius: 0.33, oddDivide: false));
                            break;
                        case "PurePlus":
                            colors.AddRange(GenerateColors(9, globalStart, globalEnd, radius: 0.33));
                            break;
                        case "Default":
                            colors.AddRange(GenerateColors(ledCount, globalStart, globalEnd));
                            break;
                        default:
                            break;
                    }

                    result.Add(port, colors);
                }
            }

            return result;
        }

        private List<LedColor> GenerateColors(int ledCount, double globalStart, double globalEnd, double radius = 1.0, bool oddDivide = true)
        {
            var colors = Enumerable.Range(0, ledCount).Select(_ => new LedColor()).ToList();

            var tBottom = _t - Config.Height / 2;
            var tTop = _t + Config.Height / 2;
            var localStart = (tBottom - globalStart) / (globalEnd - globalStart);
            var localEnd = (tTop - globalStart) / (globalEnd - globalStart);

            var isOdd = ledCount % 2 != 0;
            var halfCount = ledCount / 2 + (oddDivide || isOdd ? 0 : -1);
            for (var j = 0; j <= halfCount; j++)
            {
                var a = (0.5 + j / (double)halfCount) * Math.PI;
                var x = -Math.Cos(a) / 2 * radius;
                var y = 1 - (1 + Math.Sin(a) * radius) / 2;

                if (x > Config.Width / 2)
                    continue;

                if (y >= localStart && y <= localEnd)
                {
                    var color = Config.ColorGradient.GetColor(globalStart + (globalEnd - globalStart) * y);
                    if (Config.EnableSmoothing)
                    {
                        var dist = Math.Abs(Math.Min(y - localStart, localEnd - y));
                        var falloff = (2 * dist) / (localEnd - localStart);

                        var (h, s, v) = LedColor.ToHsv(color);
                        color = LedColor.FromHsv(h, s, v * falloff);
                    }

                    colors[j] = color;
                    if (!oddDivide && !isOdd)
                        colors[ledCount - j - 1] = color;
                    else if (j != 0 && (j != ledCount / 2 || isOdd))
                        colors[ledCount - j] = color;
                }
            }

            return colors;
        }
    }
}
