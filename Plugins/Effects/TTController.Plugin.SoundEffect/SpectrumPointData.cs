namespace TTController.Plugin.SoundEffect
{
    public class SpectrumPointData
    {
        public int SpectrumPointIndex { get; }
        public double Value { get; }

        public SpectrumPointData(int spectrumPointIndex, double value)
        {
            SpectrumPointIndex = spectrumPointIndex;
            Value = value;
        }
    }
}
