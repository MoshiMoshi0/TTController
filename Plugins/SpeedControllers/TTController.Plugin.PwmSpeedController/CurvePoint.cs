namespace TTController.Plugin.PwmSpeedController
{
    public struct CurvePoint
    {
        public int Value { get; }
        public int Speed { get; }

        public CurvePoint(int value, int speed)
        {
            Value = value;
            Speed = speed;
        }

        public override string ToString() => $"[{Value}°C, {Speed}%]";
    }
}
