using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace TTController.Common.Plugin
{
    public interface ISpeedControllerBase : IDisposable
    {
        bool IsEnabled(ICacheProvider cache);
        IEnumerable<Identifier> UsedSensors { get; }
        IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }

    public abstract class SpeedControllerConfigBase
    {
        [DefaultValue(null)] public ITriggerBase Trigger { get; private set; } = null;
    }

    public abstract class SpeedControllerBase<T> : ISpeedControllerBase where T : SpeedControllerConfigBase
    {
        protected T Config { get; private set; }
        public IEnumerable<Identifier> UsedSensors { get; private set; }

        protected SpeedControllerBase(T config) : this(config, Enumerable.Empty<Identifier>()) { }

        protected SpeedControllerBase(T config, IEnumerable<Identifier> usedSensors)
        {
            Config = config;
            UsedSensors = usedSensors
                .Union(config?.Trigger?.UsedSensors ?? Enumerable.Empty<Identifier>())
                .ToList();
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
            UsedSensors = null;
        }

        public abstract IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }
}
