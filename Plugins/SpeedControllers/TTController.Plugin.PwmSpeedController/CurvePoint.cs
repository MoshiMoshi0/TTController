namespace TTController.Plugin.PwmSpeedController
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

        public override string ToString() => $"[{Temperature}°C, {Speed}%]";
    }
}
