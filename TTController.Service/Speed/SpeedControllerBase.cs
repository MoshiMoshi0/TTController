using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TTController.Common;
using TTController.Service.Manager;

namespace TTController.Service.Speed
{
    public interface ISpeedControllerBase : IDisposable
    {
        bool Enabled { get; }
        IDictionary<PortIdentifier, byte> GenerateSpeeds(IDictionary<PortIdentifier, PortData> portDataMap);
    }

    public abstract class SpeedControllerBase<T> : ISpeedControllerBase where T : SpeedControllerConfigBase
    {
        //TODO: this should be a proxy
        protected TemperatureManager TemperatureManager { get; }
        protected T Config { get; }
        public virtual bool Enabled => Config.Trigger.Value();

        protected SpeedControllerBase(TemperatureManager temperatureManager, dynamic rawConfig)
        {
            TemperatureManager = temperatureManager;
            Config = JsonConvert.DeserializeObject(rawConfig.ToString(), typeof(T));
        }

        public virtual void Dispose() { }

        public abstract IDictionary<PortIdentifier, byte> GenerateSpeeds(IDictionary<PortIdentifier, PortData> portDataMap);
    }
}
