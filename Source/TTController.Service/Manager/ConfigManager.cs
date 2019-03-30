using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using TTController.Service.Config;
using TTController.Service.Config.Data;
using TTController.Service.Utils;

namespace TTController.Service.Manager
{
    public class ConfigManager : IDataProvider, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _filename;
        
        public ConfigData CurrentConfig { private set; get; }

        public ConfigManager(string filename)
        {
            Logger.Info("Creating Config Manager...");
            _filename = filename;

            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Formatting = Formatting.Indented,
                    Culture = CultureInfo.InvariantCulture,
                    ContractResolver = new ContractResolver()
                };

                var converters = typeof(JsonConverter).FindInAssemblies()
                    .Where(t => t.Namespace?.StartsWith("TTController") ?? false)
                    .Where(t => !t.IsGenericType && !t.IsAbstract)
                    .Select(t => (JsonConverter) Activator.CreateInstance(t));

                settings.Converters.Add(new StringEnumConverter());

                foreach (var converter in converters)
                    settings.Converters.Add(converter);

                return settings;
            };
        }

        public void SaveConfig()
        {
            Logger.Info("Saving config...");
            using (var writer = new StreamWriter(GetConfigAbsolutePath(), false))
            {
                writer.Write(JsonConvert.SerializeObject(CurrentConfig));
            }
            Logger.Info("Saving done...");
        }

        public ConfigData LoadOrCreateConfig()
        {
            Logger.Info("Loading config...");
            var path = GetConfigAbsolutePath();
            if (!File.Exists(path))
            {
                Logger.Warn("Config does not exist! Creating default config...");
                CurrentConfig = ConfigData.CreateDefault();
                SaveConfig();
            }
            else
            {
                using (var reader = new StreamReader(path))
                {
                    CurrentConfig =
                        JsonConvert.DeserializeObject<ConfigData>(reader.ReadToEnd()) ??
                        ConfigData.CreateDefault();
                }
            }

            Logger.Info("Loading done...");
            return CurrentConfig;
        }


        private string GetConfigAbsolutePath()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return Path.Combine(directory, _filename);
        }

        public void Accept(ICacheCollector collector)
        {
            foreach (var (port, config) in CurrentConfig.PortConfig)
                collector.StorePortConfig(port, config);
        }

        public void Dispose()
        {
            Logger.Info("Disposing ConfigManager...");
        }
    }
}
