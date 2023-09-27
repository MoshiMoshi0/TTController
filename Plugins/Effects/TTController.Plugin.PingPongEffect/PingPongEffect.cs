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
        [DefaultValue(0.01f)] public float Step { get; internal set; } = 0.01f;
        [DefaultValue(0.2f)] public float Height { get; internal set; } = 0.2f;
        [DefaultValue(0.5f)] public float Width { get; internal set; } = 0.5f;
        public LedColorGradient ColorGradient { get; internal set; } = new LedColorGradient();
        [DefaultValue(true)] public bool EnableSmoothing { get; internal set; } = true;
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
                _direction = -_direction;

            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            for (var i = 0; i < ports.Count; i++)
            {
                var port = ports[i];
                var config = cache.GetDeviceConfig(port);
                var ledCount = config.LedCount;

                var portStart = i / (double)ports.Count;
                var portEnd = (i + 1) / (double)ports.Count;

                var tBottom = _t - Config.Height / 2;
                var tTop = _t + Config.Height / 2;
                if ((tBottom < portStart && tTop < portStart) || (tBottom > portEnd && tTop > portEnd))
                {
                    result.Add(port, Enumerable.Range(0, ledCount).Select(_ => new LedColor()).ToList());
                }
                else if ((tBottom >= portStart && tBottom <= portEnd)
                    || (tTop >= portStart && tTop <= portEnd)
                    || (tBottom < portStart && tTop > portEnd))
                {
                    var colors = new List<LedColor>();
                    switch (config.Name)
                    {
                        case "RiingTrio":
                            colors.AddRange(GenerateColors(12, portStart, portEnd));
                            colors.AddRange(colors.ToList());
                            colors.AddRange(GenerateColors(6, portStart, portEnd, radius: 0.33, oddDivide: false));
                            break;
                        case "RiingDuo":
                            colors.AddRange(GenerateColors(12, portStart, portEnd));
                            colors.AddRange(GenerateColors(6, portStart, portEnd, radius: 0.33, oddDivide: false));
                            break;
                        case "Toughfan":
                            colors.AddRange(GenerateColors(12, portStart, portEnd));
                            colors.AddRange(colors.ToList());
                            colors.AddRange(GenerateColors(6, portStart, portEnd, radius: 0.33, oddDivide: false));
                            break;
                        case "PurePlus":
                            colors.AddRange(GenerateColors(9, portStart, portEnd, radius: 0.33));
                            break;
                        case "Default":
                            colors.AddRange(GenerateColors(ledCount, portStart, portEnd));
                            break;
                        default:
                            break;
                    }

                    result.Add(port, colors);
                }
            }

            return result;
        }

        private List<LedColor> GenerateColors(int ledCount, double portStart, double portEnd, double radius = 0.95, bool oddDivide = true)
        {
            var colors = Enumerable.Range(0, ledCount).Select(_ => new LedColor()).ToList();

            var tBottom = _t - Config.Height / 2;
            var tTop = _t + Config.Height / 2;
            var localStart = (tBottom - portStart) / (portEnd - portStart);
            var localEnd = (tTop - portStart) / (portEnd - portStart);

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
                    var color = Config.ColorGradient.GetColor(portStart + (portEnd - portStart) * y);
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

        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
            => throw new NotImplementedException();
    }
}
