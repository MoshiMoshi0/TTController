using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Service.Hardware.Sensor
{
    public class OffsetSensorValueDecorator : AbstractSensorValueDecorator
    {
        private readonly float _offset;

        public OffsetSensorValueDecorator(ISensorValueProvider sensorValueProvider, float offset)
            : base(sensorValueProvider)
        {
            _offset = offset;

            CurrentValue = null;
        }

        public override void Update()
        {
            SensorValueProvider.Update();
            var newValue = SensorValueProvider.ValueOrDefault(float.NaN);
            if (float.IsNaN(newValue))
                return;

            CurrentValue = newValue + _offset;
        }
    }
}
