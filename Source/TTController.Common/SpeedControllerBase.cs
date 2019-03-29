using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OpenHardwareMonitor.Hardware;

namespace TTController.Common
{
    public interface ISpeedControllerBase : IDisposable
    {
        bool Enabled { get; }
        IEnumerable<Identifier> UsedSensors { get; }
        IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }

    public abstract class SpeedControllerConfigBase
    {
        [DefaultValue(null)] public ITriggerBase Trigger { get; private set; } = null;
    }

    public abstract class SpeedControllerBase<T> : ISpeedControllerBase where T : SpeedControllerConfigBase
    {
        public T Config { get; }
        public virtual bool Enabled => Config.Trigger?.Value() ?? false;
        public virtual IEnumerable<Identifier> UsedSensors => Enumerable.Empty<Identifier>();

        protected SpeedControllerBase(T config)
        {
            Config = config;
        }

        public virtual void Dispose() { }

        public abstract IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache);
    }
}
