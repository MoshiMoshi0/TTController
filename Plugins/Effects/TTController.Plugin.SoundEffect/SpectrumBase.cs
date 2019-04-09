using System;
using System.Collections.Generic;
using CSCore;
using CSCore.DSP;

namespace TTController.Plugin.SoundEffect
{
    public abstract class SpectrumBase
    {
        private int _fftSize;
        private int _maxFftIndex;
        private int _maximumFrequencyIndex;
        private int _minimumFrequencyIndex;
        private int[] _spectrumIndexMax;

        public int MinDbValue { get; set; } = -90;
        public int MaxDbValue { get; set; } = 0;
        public int DbScale => MaxDbValue - MinDbValue;
        public int SpectrumResolution { get; set; }
        public double ScalingFactor { get; set; } = 2.0;
        public int MaximumFrequency { get; set; } = 20000;
        public int MinimumFrequency { get; set; } = 20;
        public ISpectrumProvider SpectrumProvider { get; set; }
        public bool IsXLogScale { get; set; }
        public ScalingStrategy ScalingStrategy { get; set; }
        public bool UseAverage { get; set; }

        public FftSize FftSize
        {
            get => (FftSize)_fftSize;
            set
            {
                _fftSize = (int)value;
                _maxFftIndex = _fftSize / 2 - 1;
            }
        }

        protected SpectrumBase() { }
        
        public virtual void UpdateFrequencyMapping()
        {
            _maximumFrequencyIndex = Math.Min(SpectrumProvider.GetFftBandIndex(MaximumFrequency) + 1, _maxFftIndex);
            _minimumFrequencyIndex = Math.Min(SpectrumProvider.GetFftBandIndex(MinimumFrequency), _maxFftIndex);

            var actualResolution = SpectrumResolution;
            var indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
            var linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

            _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, true);
            for (var i = 1; i < actualResolution; i++)
            {
                if (!IsXLogScale)
                    _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
                else
                    _spectrumIndexMax[i - 1] =
                        (int)((Math.Log(actualResolution, actualResolution) - 
                               Math.Log(actualResolution + 1 - i, actualResolution + 1))
                              * indexCount + _minimumFrequencyIndex);
            }

            if (actualResolution > 0)
                _spectrumIndexMax[_spectrumIndexMax.Length - 1] = _maximumFrequencyIndex;
        }

        protected List<SpectrumPointData> CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
        {
            var dataPoints = new List<SpectrumPointData>();

            var value = 0.0;
            var value0 = 0.0;
            var lastValue = 0.0;
            var actualMaxValue = maxValue;
            var spectrumPointIndex = 0;

            for (var i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
            {
                switch (ScalingStrategy)
                {
                    case ScalingStrategy.Decibel:
                        value0 = ((20 * Math.Log10(fftBuffer[i]) - MinDbValue) / DbScale) * actualMaxValue;
                        break;
                    case ScalingStrategy.Linear:
                        value0 = fftBuffer[i] * ScalingFactor * actualMaxValue;
                        break;
                    case ScalingStrategy.Sqrt:
                        value0 = Math.Sqrt(fftBuffer[i]) * ScalingFactor * actualMaxValue;
                        break;
                }

                var recalc = true;
                value = Math.Max(0, Math.Max(value0, value));

                while (spectrumPointIndex <= _spectrumIndexMax.Length - 1 && i == _spectrumIndexMax[spectrumPointIndex])
                {
                    if (!recalc)
                        value = lastValue;

                    if (value > maxValue)
                        value = maxValue;

                    if (UseAverage && spectrumPointIndex > 0)
                        value = (lastValue + value) / 2.0;

                    dataPoints.Add(new SpectrumPointData(spectrumPointIndex, value));

                    lastValue = value;
                    value = 0.0;
                    spectrumPointIndex++;
                    recalc = false;
                }
            }

            return dataPoints;
        }
        
        protected struct SpectrumPointData
        {
            public int SpectrumPointIndex { get; private set; }
            public double Value { get; private set; }

            public SpectrumPointData(int spectrumPointIndex, double value)
            {
                SpectrumPointIndex = spectrumPointIndex;
                Value = value;
            }
        }
    }

    public enum ScalingStrategy
    {
        Decibel,
        Linear,
        Sqrt
    }
}
