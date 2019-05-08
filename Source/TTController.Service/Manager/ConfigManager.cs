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
    public sealed class ConfigManager : IDataProvider, IDisposable
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
                settings.Error += (sender, args) =>
                {
                    if(args.CurrentObject == args.ErrorContext.OriginalObject)
                        Logger.Fatal(args.ErrorContext.Error.Message);
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

        public bool SaveConfig()
        {
            Logger.Info("Saving config...");
            using (var writer = new StreamWriter(GetConfigAbsolutePath(), false))
            {
                try
                {
                    writer.Write(JsonConvert.SerializeObject(CurrentConfig));
                }
                catch (Exception e)
                {
                    if (!(e is JsonWriterException))
                        Logger.Fatal(e);

                    Logger.Fatal("Failed to save the config file!");
                    return false;
                }
            }

            Logger.Info("Saving done...");
            return true;
        }

        public bool LoadOrCreateConfig()
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
                try
                {
                    using (var reader = new StreamReader(path))
                        CurrentConfig = JsonConvert.DeserializeObject<ConfigData>(reader.ReadToEnd());
                }
                catch (Exception e)
                {
                    if (!(e is JsonReaderException))
                        Logger.Fatal(e);
                }

                if (CurrentConfig == null)
                {
                    Logger.Fatal("Failed to load the config file!");
                    return false;
                }
            }

            Logger.Info("Loading done...");
            return true;
        }

        private string GetConfigAbsolutePath()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return Path.Combine(directory, _filename);
        }

        public void Accept(ICacheCollector collector)
        {
            foreach (var (ports, config) in CurrentConfig.PortConfigs)
                foreach (var port in ports)
                    collector.StorePortConfig(port, config);

            foreach (var (sensors, config) in CurrentConfig.SensorConfigs)
                foreach (var sensor in sensors)
                    collector.StoreSensorConfig(sensor, config);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing ConfigManager...");

            CurrentConfig = null;
        }
    }
}
