using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using LibreHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.StaticColorEffect2
{
    public class StaticColorEffect2Config : EffectConfigBase
    {
        public List<CurvePoint> CurvePoints { get; internal set; } = new List<CurvePoint>();
        public List<Identifier> Sensors { get; internal set; } = new List<Identifier>();

        [DefaultValue(SensorMixFunction.Maximum)]
        public SensorMixFunction SensorMixFunction { get; internal set; } = SensorMixFunction.Maximum;

        [DefaultValue(4)] public int MinimumChange { get; internal set; } = 4;
        [DefaultValue(8)] public int MaximumChange { get; internal set; } = 8;
    }

    public class StaticColorEffect2 : EffectBase<StaticColorEffect2Config>
    {
        public StaticColorEffect2(StaticColorEffect2Config config) : base(config)
        {
        }

        public override string EffectType => "PerLed";

        private Color InterpolateColor(double temperature)
        {
            var minTemperature = Config.CurvePoints.Min(x => x.Value);
            var maxTemperature = Config.CurvePoints.Max(x => x.Value);
            var minConfig = Config.CurvePoints.First(x => x.Value == minTemperature);
            var maxConfig = Config.CurvePoints.First(x => x.Value == maxTemperature);
            var lowerPoint = Color.FromArgb(minConfig.Red, minConfig.Green, minConfig.Blue);
            var upperPoint = Color.FromArgb(maxConfig.Red, maxConfig.Green, maxConfig.Blue);

            for (int i = 0; i < Config.CurvePoints.Count - 1; i++)
            {
                if (Config.CurvePoints[i].Value <= temperature && temperature <= Config.CurvePoints[i + 1].Value)
                {
                    lowerPoint = Color.FromArgb(Config.CurvePoints[i].Red, Config.CurvePoints[i].Green,
                        Config.CurvePoints[i].Blue);
                    upperPoint = Color.FromArgb(Config.CurvePoints[i + 1].Red, Config.CurvePoints[i + 1].Green,
                        Config.CurvePoints[i + 1].Blue);
                    break;
                }
            }

            var t = (temperature - minTemperature) / (maxTemperature - minTemperature);

            var r = (int)(lowerPoint.R + t * (upperPoint.R - lowerPoint.R));
            var g = (int)(lowerPoint.G + t * (upperPoint.G - lowerPoint.G));
            var b = (int)(lowerPoint.B + t * (upperPoint.B - lowerPoint.B));

            return Color.FromArgb(r, g, b);
        }


        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports,
            ICacheProvider cache)
        {
            var values = Config.Sensors.Select(cache.GetSensorValue);
            var value = float.NaN;
            if (Config.SensorMixFunction == SensorMixFunction.Average)
                value = values.Average();
            else if (Config.SensorMixFunction == SensorMixFunction.Minimum)
                value = values.Min();
            else if (Config.SensorMixFunction == SensorMixFunction.Maximum)
                value = values.Max();

            if (float.IsNaN(value))
                return ports.ToDictionary(p => p, _ => new List<LedColor> { new LedColor(255, 255, 255) });

            var curveTargetColor = InterpolateColor(value);
            
            return ports.ToDictionary(p => p, _ => new List<LedColor>
                { new LedColor(curveTargetColor.R, curveTargetColor.G, curveTargetColor.B) }
            );
        }


        public override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            return null;
        }
    }
}