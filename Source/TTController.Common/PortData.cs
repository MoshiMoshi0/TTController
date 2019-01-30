using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common
{
    public class PortData
    {
        public byte Id { get; }
        public byte Unknown { get; }
        public byte Speed { get; }
        public int Rpm { get; }

        public PortData(byte id, byte unknown, byte speed, int rpm)
        {
            Id = id;
            Unknown = unknown;
            Speed = speed;
            Rpm = rpm;
        }

        public override string ToString() => $"[{Id}, 0x{Unknown:x}, {Speed}%, {Rpm} RPM]";
    }
}
