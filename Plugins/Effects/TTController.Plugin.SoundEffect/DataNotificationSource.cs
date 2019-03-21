using System;
using CSCore;

namespace TTController.Plugin.SoundEffect
{
    public class DataNotificationSource : SampleAggregatorBase
    {
        public event EventHandler<DataReadEventArgs> DataRead;

        public DataNotificationSource(ISampleSource source) : base(source) { }

        public override int Read(float[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            DataRead?.Invoke(this, new DataReadEventArgs(buffer, offset, read));
            return read;
        }
    }

    public class DataReadEventArgs
    {
        public float[] Data { get; }

        public DataReadEventArgs(float[] buffer, int offset, int count)
        {
            Data = new float[count];
            Array.Copy(buffer, offset, Data, 0, count);
        }
    }
}
