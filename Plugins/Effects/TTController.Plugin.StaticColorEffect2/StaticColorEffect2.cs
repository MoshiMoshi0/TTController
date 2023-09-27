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

            var curveTargetColor = Color.FromArgb(255, 255, 255, 255); // Default color (e.g., gray)

            for (var i = 0; i <= Config.CurvePoints.Count; i++)
            {
                var current = i == Config.CurvePoints.Count
                    ? new CurvePoint(100, Config.CurvePoints[i - 1].Red, Config.CurvePoints[i - 1].Green,
                        Config.CurvePoints[i - 1].Blue)
                    : Config.CurvePoints[i];

                if (value >= current.Value)
                    continue;

                var last = i == 0
                    ? new CurvePoint(0, current.Red, current.Green, current.Blue)
                    : Config.CurvePoints[i - 1];

                var t = (value - last.Value) / (current.Value - last.Value);
                var red = (byte)Math.Round(last.Red * (1 - t) + current.Red * t);
                var green = (byte)Math.Round(last.Green * (1 - t) + current.Green * t);
                var blue = (byte)Math.Round(last.Blue * (1 - t) + current.Blue * t);

                curveTargetColor = Color.FromArgb(255, red, green, blue);

                break;
            }

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