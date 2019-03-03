using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using TTController.Common;
using TTController.Service.Config;
using TTController.Service.Config.Converter;
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

                settings.Converters.Add(new StringEnumConverter());
                settings.Converters.Add(new EffectConverter());
                settings.Converters.Add(new SpeedControllerConverter());
                settings.Converters.Add(new TriggerConverter());
                settings.Converters.Add(new PortIdentifierConverter());
                settings.Converters.Add(new CurvePointConverter());
                settings.Converters.Add(new LedColorConverter());
                settings.Converters.Add(new IdentifierConverter());
                settings.Converters.Add(new IdentifierDictionaryConverter<int>());

                return settings;
            };
        }

        public void SaveConfig()
        {
            Logger.Info("Saving conifg...");
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
                Logger.Warn("Config does not exist! Creating default conifg...");
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

        public void Visit(ICacheCollector collector)
        {
            foreach (var (port, config) in CurrentConfig.PortConfig)
                collector.StorePortConfig(port, config);
        }

        public void Dispose() { }
    }
}
