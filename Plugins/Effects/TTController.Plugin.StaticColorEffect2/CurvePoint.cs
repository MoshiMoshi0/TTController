using System.Drawing;

namespace TTController.Plugin.StaticColorEffect2
{
    public struct CurvePoint
    {
        public int Value { get; }
        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }

        public CurvePoint(int value, byte red, byte green, byte blue)
        {
            Value = value;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString() => $"[Value: {Value}, Color: ({Red}, {Green}, {Blue})]";
    }
}