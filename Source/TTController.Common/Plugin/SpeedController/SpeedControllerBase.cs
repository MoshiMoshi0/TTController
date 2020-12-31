using System;
using System.Collections.Generic;
using System.Linq;
using LibreHardwareMonitor.Hardware;
using NLog;

namespace TTController.Common.Plugin
{
    public abstract class SpeedControllerBase<T> : ISpeedControllerBase where T : SpeedControllerConfigBase
    {
        protected T Config { get; private set; }

        protected SpeedControllerBase(T config)
        {
            Config = config;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual bool IsEnabled(ICacheProvider cache) => Config.Trigger?.Value(cache) ?? false;

        protected virtual void Dispose(bool disposing)
        {
            Config = null;
        }

        public IDictionary<PortIdentifier, byte> GetSpeeds(List<PortIdentifier> ports, ICacheProvider cache) => GenerateSpeeds(ports, cache);

        protected abstract IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }
}
