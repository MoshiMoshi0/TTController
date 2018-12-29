using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Service.Manager;

namespace TTController.Service.Speed.Controller
{
    public enum SensorMixFunction
    {
        Minimum,
        Maximum,
        Average
    };

    public class PwmSpeedControllerConfig : SpeedControllerConfigBase
    {
        public List<CurvePoint> CurvePoints { get; set; }
        public List<Identifier> Sensors { get; set; }
        public SensorMixFunction SensorMixFunction { get; set; } = SensorMixFunction.Maximum;
        public int MinimumChange { get; set; } = 4;
        public int MaximumChange { get; set; } = 8;
    }

    public class PwmSpeedController : SpeedControllerBase<PwmSpeedControllerConfig>
    {
        public PwmSpeedController(TemperatureManager temperatureManager, dynamic config) :
            base(temperatureManager, (object) config)
        {
        }

        public override IDictionary<PortIdentifier, byte> GenerateSpeeds(IDictionary<PortIdentifier, PortData> portDataMap)
        {
            var temperatures = Config.Sensors.Select(s => TemperatureManager.GetSensorValue(s));
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
                if (!(temperature < current.Temperature))
                    continue;

                var last = i == 0 ? new CurvePoint(0, current.Speed) : Config.CurvePoints[i - 1];

                var span = current.Temperature - last.Temperature;
                var t = (temperature - last.Temperature) / span;
                curveTargetSpeed = (byte) Math.Round(last.Speed * (1 - t) + current.Speed * t);

                break;
            }

            var result = new Dictionary<PortIdentifier, byte>();
            foreach (var pair in portDataMap)
            {
                if (pair.Value == null)
                {
                    result.Add(pair.Key, 100);
                    continue;
                }

                var currentSpeed = pair.Value.Speed;
                var targetSpeed = currentSpeed;
                var speedDiff = curveTargetSpeed - currentSpeed;

                if (Math.Abs(speedDiff) >= Config.MinimumChange)
                {
                    targetSpeed = (byte) (currentSpeed +
                                          Math.Sign(speedDiff) * Math.Min(Config.MaximumChange, Math.Abs(speedDiff)));

                    if (targetSpeed < 20)
                        targetSpeed = curveTargetSpeed == 0 ? (byte)0 : (byte)20;
                }

                result.Add(pair.Key, targetSpeed);
            }

            return result;
        }
    }
}