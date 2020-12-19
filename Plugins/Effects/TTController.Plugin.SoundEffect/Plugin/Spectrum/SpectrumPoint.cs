namespace TTController.Plugin.SoundEffect
{
    public readonly struct SpectrumPoint
    {
        public int Index { get; }
        public float Value { get; }

        public SpectrumPoint(int index, float value)
        {
            Index = index;
            Value = value;
        }
    }
}