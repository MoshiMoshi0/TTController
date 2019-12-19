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
            if (_t < 0)
            {
                _direction = 1;
                _t = 0;
            }
            else if (_t > 1)
            {
                _direction = -1;
                _t = 1;
            }

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
                    var localStart = (tBottom - globalStart) / (double)(globalEnd - globalStart);
                    var localEnd = (tTop - globalStart) / (double)(globalEnd - globalStart);

                    var colors = new List<LedColor>();
                    switch (config.Name)
                    {
                        case "RiingTrio":
                            colors.AddRange(GenerateColors(12, localStart, localEnd));
                            colors.AddRange(colors);
                            colors.AddRange(GenerateColors(6, localStart, localEnd, radius: 0.33, oddDivide: false));
                            break;
                        case "RiingDuo":
                            colors.AddRange(GenerateColors(12, localStart, localEnd));
                            colors.AddRange(GenerateColors(6, localStart, localEnd, radius: 0.33, oddDivide: false));
                            break;
                        case "PurePlus":
                            colors.AddRange(GenerateColors(9, localStart, localEnd, radius: 0.33));
                            break;
                        case "Default":
                            colors.AddRange(GenerateColors(ledCount, localStart, localEnd));
                            break;
                        default:
                            break;
                    }

                    result.Add(port, colors);
                }
            }

            return result;
        }

        private List<LedColor> GenerateColors(int ledCount, double localStart, double localEnd, double radius = 1.0, bool oddDivide = true)
        {
            var colors = Enumerable.Range(0, ledCount).Select(_ => new LedColor()).ToList();

            var isOdd = ledCount % 2 != 0;
            var halfCount = ledCount / 2 + (oddDivide || isOdd ? 0 : -1);
            for (var j = 0; j <= halfCount; j++)
            {
                var a = (0.5 + j / (double)halfCount) * Math.PI;
                var x = -Math.Cos(a) / 2 * radius;
                var y = 1 - (1 + Math.Sin(a) * radius) / 2;

                if (x >= Config.Width / 2)
                    continue;

                if (y >= localStart && y <= localEnd)
                {
                    var dist = Math.Abs(Math.Min(y - localStart, localEnd - y));
                    var falloff = (2 * dist) / (localEnd - localStart);
                    var brightness = (byte)(255 * falloff);
                    var color = new LedColor(brightness, brightness, brightness);

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
