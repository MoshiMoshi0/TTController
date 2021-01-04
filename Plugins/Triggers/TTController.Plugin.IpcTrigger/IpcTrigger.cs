using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Threading.Channels;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.IpcTrigger
{
    public class IpcTriggerConfig : TriggerConfigBase
    {
        [DefaultValue(null)] public string IpcName { get; internal set; } = null;
        [DefaultValue(false)] public bool EnabledByDefault { get; internal set; } = false;
    }

    public class IpcTrigger : IpcTriggerBase<IpcTriggerConfig>
    {
        private bool _enabled;

        public override string IpcName => Config.IpcName;

        public IpcTrigger(IpcTriggerConfig config) : base(config)
        {
            _enabled = Config.EnabledByDefault;
        }

        public override bool Value(ICacheProvider cache) => _enabled;

        protected override void OnDataReceived(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return;

            try
            {
                var document = JObject.Parse(data);
                _enabled = document["Enabled"].ToObject<bool>();
            }
            catch (JsonReaderException) { }
        }
    }
}
