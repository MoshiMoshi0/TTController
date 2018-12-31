using System;
using System.Collections.Generic;
using TTController.Common;
using TTController.Service.Manager;

namespace TTController.Service.Speed
{
    public interface ISpeedControllerBase : IDisposable
    {
        bool Enabled { get; }
        IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }

    public abstract class SpeedControllerBase<T> : ISpeedControllerBase where T : SpeedControllerConfigBase
    {
        public T Config { get; }
        public virtual bool Enabled => Config.Trigger?.Value() ?? false;

        protected SpeedControllerBase(T config)
        {
            Config = config;
        }

        public virtual void Dispose() { }

        public abstract IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }
}
