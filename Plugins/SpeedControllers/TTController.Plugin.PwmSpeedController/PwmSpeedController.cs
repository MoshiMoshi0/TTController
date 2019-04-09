using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OpenHardwareMonitor.Hardware;
using TTController.Common;

namespace TTController.Plugin.PwmSpeedController
{
    public class PwmSpeedControllerConfig : SpeedControllerConfigBase
    {
        public List<CurvePoint> CurvePoints { get; private set; } = new List<CurvePoint>();
        public List<Identifier> Sensors { get; private set; } = new List<Identifier>();
        [DefaultValue(SensorMixFunction.Maximum)] public SensorMixFunction SensorMixFunction { get; private set; } = SensorMixFunction.Maximum;
        [DefaultValue(4)] public int MinimumChange { get; private set; } = 4;
        [DefaultValue(8)] public int MaximumChange { get; private set; } = 8;
    }

    public class PwmSpeedController : SpeedControllerBase<PwmSpeedControllerConfig>
    {
        public PwmSpeedController(PwmSpeedControllerConfig config) : base(config, config.Sensors) { }

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
        {
            var temperatures = Config.Sensors.Select(cache.GetTemperature);
            var temperature = float.NaN;
            if (Config.SensorMixFunction == SensorMixFunction.Average)
                temperature = temperatures.Average();
            else if (Config.SensorMixFunction == SensorMixFunction.Minimum)
                temperature = temperatures.Min();
            else if (Config.SensorMixFunction == SensorMixFunction.Maximum)
                temperature = temperatures.Max();

            byte curveTargetSpeed = 100;
            for (var i = 0; i <= Config.CurvePoints.Count; i++)
            {
                var current = i == Config.CurvePoints.Count
                    ? new CurvePoint(100, Config.CurvePoints[i - 1].Speed)
                    : Config.CurvePoints[i];
                if (temperature >= current.Temperature)
                    continue;

                var last = i == 0 ? new CurvePoint(0, current.Speed) : Config.CurvePoints[i - 1];

                var span = current.Temperature - last.Temperature;
                var t = (temperature - last.Temperature) / span;
                curveTargetSpeed = (byte) Math.Round(last.Speed * (1 - t) + current.Speed * t);

                break;
            }

            var port = ports.First(p => cache.GetPortData(p).Speed.HasValue);
            var currentSpeed = cache.GetPortData(port).Speed.Value;
            var targetSpeed = currentSpeed;
            var speedDiff = curveTargetSpeed - currentSpeed;

            if (Math.Abs(speedDiff) >= Config.MinimumChange || curveTargetSpeed >= 100 || curveTargetSpeed <= 20)
            {
                targetSpeed = (byte) (currentSpeed + Math.Sign(speedDiff) * Math.Min(Config.MaximumChange, Math.Abs(speedDiff)));

                if (targetSpeed < 20)
                    targetSpeed = curveTargetSpeed == 0 ? (byte) 0 : (byte) 20;
                else if (targetSpeed > 100)
                    targetSpeed = 100;
            }

            return ports.ToDictionary(p => p, _ => targetSpeed);
        }
    }
}