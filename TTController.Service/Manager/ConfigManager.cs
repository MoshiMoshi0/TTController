using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using TTController.Common.Config;
using TTController.Service.Config.Converter;

namespace TTController.Service.Manager
{
    public class ConfigManager
    {
        private readonly string _filename;
        private readonly JsonSerializerSettings _serializerSettings;
        
        public ConfigData CurrentConfig { private set; get; }

        public ConfigManager(string filename)
        {
            _filename = filename;

            _serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture
            };
            _serializerSettings.Converters.Add(new PortIdentifierConverter());
        }

        public void SaveConfig()
        {
            using (var writer = new StreamWriter(GetConfigAbsolutePath(), false))
            {
                lock (CurrentConfig)
                {
                    writer.Write(JsonConvert.SerializeObject(CurrentConfig, _serializerSettings));
                }
            }
        }

        public ConfigData LoadOrCreateConfig()
        {
            var path = GetConfigAbsolutePath();
            if (!File.Exists(path))
            {
                CurrentConfig = ConfigData.CreateDefault();
                SaveConfig();
            }
            else
            {
                using (var reader = new StreamReader(GetConfigAbsolutePath()))
                {
                    lock (CurrentConfig)
                    {
                        CurrentConfig =
                            JsonConvert.DeserializeObject<ConfigData>(reader.ReadToEnd(), _serializerSettings) ??
                            ConfigData.CreateDefault();
                    }
                }
            }

            return CurrentConfig;
        }


        private string GetConfigAbsolutePath()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return Path.Combine(directory, _filename);
        }
    }
}
