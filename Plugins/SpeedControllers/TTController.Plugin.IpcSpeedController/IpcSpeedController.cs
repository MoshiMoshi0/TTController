using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.IpcSpeedController
{
    public class IpcSpeedControllerConfig : SpeedControllerConfigBase
    {
        [DefaultValue(null)] public string IpcName { get; internal set; } = null;
        [DefaultValue(50)] public byte DefaultSpeed { get; internal set; } = 50;
    }

    public class IpcSpeedController : IpcSpeedControllerBase<IpcSpeedControllerConfig>
    {
        private readonly Dictionary<PortIdentifier, byte> _speedMap;

        public override string IpcName => Config.IpcName;

        public IpcSpeedController(IpcSpeedControllerConfig config) : base(config)
        {
            _speedMap = new Dictionary<PortIdentifier, byte>();
        }

        protected override void OnDataReceived(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return;

            try
            {
                var document = JArray.Parse(data);
                foreach(var child in document.Children())
                {
                    var port = child["Port"].ToObject<PortIdentifier>();
                    _speedMap[port] = child["Speed"].ToObject<byte>();
                }
            }
            catch (JsonReaderException) { }
        }

        protected override IDictionary<PortIdentifier, byte> GenerateSpeeds(List<PortIdentifier> ports, ICacheProvider cache)
            => ports.ToDictionary(p => p, p => {
                if (_speedMap.TryGetValue(p, out var speed))
                    return speed == 0 || speed >= 20 ? speed : (byte)20;
                return Config.DefaultSpeed;
            });
    }
}
