using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using TTController.Config;

namespace TTController.Service.Managers
{
    public class ConfigManager
    {
        private readonly string _filename;
        private readonly JsonSerializerSettings _serializerSettings;

        private ConfigData _config;

        public ConfigManager(string filename)
        {
            _filename = filename;

            _serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture
            };
        }

        public void SaveConfig()
        {
            using (var writer = new StreamWriter(GetConfigAbsolutePath(), false))
            {
                lock (_config)
                {
                    writer.Write(JsonConvert.SerializeObject(_config, _serializerSettings));
                }
            }
        }

        public ConfigData LoadOrCreateConfig()
        {
            var path = GetConfigAbsolutePath();
            if (!File.Exists(path))
            {
                _config = ConfigData.CreateDefault();
                SaveConfig();
            }
            else
            {
                using (var reader = new StreamReader(GetConfigAbsolutePath()))
                {
                    _config = JsonConvert.DeserializeObject<ConfigData>(reader.ReadToEnd(), _serializerSettings);
                }
            }

            return _config;
        }


        private string GetConfigAbsolutePath()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return Path.Combine(directory, _filename);
        }
    }
}
