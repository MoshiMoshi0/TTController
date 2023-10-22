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
            var curvePoints = Config.CurvePoints.OrderBy(p => p.Value).ToArray();
            var n = curvePoints.Length;

            if (temperature <= curvePoints[0].Value)
            {
                return CreateColorFromCurvePoint(curvePoints[0]);
            }

            if (temperature >= curvePoints[n - 1].Value)
            {
                return CreateColorFromCurvePoint(curvePoints[n - 1]);
            }

            for (var i = 0; i < n - 1; i++)
            {
                if (IsInRange(temperature, curvePoints[i].Value, curvePoints[i + 1].Value))
                {
                    var t = CalculateInterpolationParameter(temperature, curvePoints[i].Value,
                        curvePoints[i + 1].Value);

                    var r = InterpolateLinear(curvePoints[i].Red, curvePoints[i + 1].Red, t);
                    var g = InterpolateLinear(curvePoints[i].Green, curvePoints[i + 1].Green, t);
                    var b = InterpolateLinear(curvePoints[i].Blue, curvePoints[i + 1].Blue, t);

                    return CreateColorFromRgbValues(r, g, b);
                }
            }

            return Color.White; // Handle cases outside the defined range
        }

        private Color CreateColorFromCurvePoint(CurvePoint curvePoint)
        {
            return Color.FromArgb(curvePoint.Red, curvePoint.Green, curvePoint.Blue);
        }

        private bool IsInRange(double value, int minValue, int maxValue)
        {
            return value >= minValue && value <= maxValue;
        }

        private double CalculateInterpolationParameter(double value, int minValue, int maxValue)
        {
            return (double)(value - minValue) / (maxValue - minValue);
        }

        private double InterpolateLinear(int startValue, int endValue, double t)
        {
            return startValue + t * (endValue - startValue);
        }

        private Color CreateColorFromRgbValues(double r, double g, double b)
        {
            return Color.FromArgb(Clamp((int)r, 0, 255), Clamp((int)g, 0, 255), Clamp((int)b, 0, 255));
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
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