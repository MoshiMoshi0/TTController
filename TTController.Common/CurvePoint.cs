using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
{
    public struct CurvePoint
    {
        public int Temperature { get; }
        public int Speed { get; }

        public CurvePoint(int temperature, int speed)
        {
            Temperature = temperature;
            Speed = speed;
        }
    }
}
