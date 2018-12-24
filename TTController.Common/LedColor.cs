namespace TTController.Common
{
    public struct LedColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public LedColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
