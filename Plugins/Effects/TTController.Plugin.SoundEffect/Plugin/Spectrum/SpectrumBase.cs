using System;
using System.Collections.Generic;
using CSCore;
using CSCore.DSP;

namespace TTController.Plugin.SoundEffect
{
    public enum ScalingStrategy
    {
        Decibel,
        Linear,
        Sqrt
    }

    public class SpectrumConfig
    {
        public FftSize FftSize { get; set; } = FftSize.Fft1024;
        public int MinDbValue { get; set; } = -90;
        public int MaxDbValue { get; set; } = 0;
        public int DbScale => MaxDbValue - MinDbValue;
        public float ScalingFactor { get; set; } = 2.0f;
        public int MaximumFrequency { get; set; } = 20000;
        public int MinimumFrequency { get; set; } = 20;
        public bool IsXLogScale { get; set; }
        public ScalingStrategy ScalingStrategy { get; set; }
        public bool UseAverage { get; set; }
    }

    public class SpectrumBase
    {
        private readonly SpectrumConfig _config;

        private readonly int _maxFftIndex;
        private int _maximumFrequencyIndex;
        private int _minimumFrequencyIndex;
        private int[] _spectrumIndexMax;

        protected ISpectrumProvider SpectrumProvider { get; private set; }

        protected int SpectrumResolution { get; set; }

        public SpectrumBase(ISpectrumProvider spectrumProvider, SpectrumConfig config)
        {
            SpectrumProvider = spectrumProvider;

            _config = config;
            _maxFftIndex = (int)_config.FftSize / 2 - 1;
        }

        public bool UpdateFrequencyMappingIfNecessary(int count)
        {
            if (count != SpectrumResolution)
            {
                SpectrumResolution = count;
                UpdateFrequencyMapping();
                return true;
            }

            return false;
        }

        public void UpdateFrequencyMapping()
        {
            _maximumFrequencyIndex = Math.Min(SpectrumProvider.GetFftBandIndex(_config.MaximumFrequency) + 1, _maxFftIndex);
            _minimumFrequencyIndex = Math.Min(SpectrumProvider.GetFftBandIndex(_config.MinimumFrequency), _maxFftIndex);

            var actualResolution = SpectrumResolution;
            var indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
            var linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

            _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, false);
            for (var i = 1; i < actualResolution; i++)
            {
                if (!_config.IsXLogScale)
                {
                    _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
                }
                else
                {
                    _spectrumIndexMax[i - 1] =
                        (int)((Math.Log(actualResolution, actualResolution)
                               - Math.Log(actualResolution + 1 - i, actualResolution + 1))
                               * indexCount + _minimumFrequencyIndex);
                }
            }

            if (actualResolution > 0)
                _spectrumIndexMax[_spectrumIndexMax.Length - 1] = _maximumFrequencyIndex;
        }

        public List<SpectrumPoint> CalculateSpectrumPoints(float maxValue, float[] fftBuffer)
        {
            var dataPoints = new List<SpectrumPoint>();

            var value = 0.0f;
            var value0 = 0.0f;
            var lastValue = 0.0f;
            var actualMaxValue = maxValue;
            var spectrumPointIndex = 0;

            for (var i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
            {
                switch (_config.ScalingStrategy)
                {
                    case ScalingStrategy.Decibel:
                        value0 = (20 * (float)Math.Log10(fftBuffer[i]) - _config.MinDbValue) / _config.DbScale * actualMaxValue;
                        break;
                    case ScalingStrategy.Linear:
                        value0 = fftBuffer[i] * _config.ScalingFactor * actualMaxValue;
                        break;
                    case ScalingStrategy.Sqrt:
                        value0 = (float)Math.Sqrt(fftBuffer[i]) * _config.ScalingFactor * actualMaxValue;
                        break;
                }

                var recalc = true;
                value = Math.Max(0, Math.Max(value0, value));

                while (spectrumPointIndex <= SpectrumResolution - 1 && i == _spectrumIndexMax[spectrumPointIndex])
                {
                    if (!recalc)
                        value = lastValue;

                    if (value > maxValue)
                        value = maxValue;

                    if (_config.UseAverage && spectrumPointIndex > 0)
                        value = (lastValue + value) / 2.0f;

                    dataPoints.Add(new SpectrumPoint(spectrumPointIndex, value));

                    lastValue = value;
                    value = 0.0f;
                    spectrumPointIndex++;
                    recalc = false;
                }
            }

            return dataPoints;
        }
    }
}
