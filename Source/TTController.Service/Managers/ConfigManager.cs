using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NLog;
using TTController.Common;
using TTController.Service.Config;
using TTController.Service.Utils;

namespace TTController.Service.Managers
{
    public sealed class ConfigManager : IDataProvider, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _filename;
        private Dictionary<string, DeviceConfig> _deviceConfigs;

        public ServiceConfig CurrentConfig { get; private set; }

        public ConfigManager(string filename, object context)
        {
            Logger.Info("Creating Config Manager...");
            _filename = filename;
            _deviceConfigs = new Dictionary<string, DeviceConfig>();

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture,
                Context = new StreamingContext(StreamingContextStates.All, context),
                ContractResolver = new ContractResolver()
            };
            jsonSettings.Error += (sender, args) => { };

            var converters = typeof(JsonConverter).FindImplementations()
                    .Where(t => (t.Namespace?.StartsWith("TTController") ?? false) && !t.IsGenericType && !t.IsAbstract)
                    .Select(t => (JsonConverter)Activator.CreateInstance(t));

            jsonSettings.Converters.Add(new StringEnumConverter());
            foreach (var converter in converters)
                jsonSettings.Converters.Add(converter);

            JsonConvert.DefaultSettings = () => jsonSettings;
        }

        public bool SaveConfig()
        {
            Logger.Info("Saving config...");
            using (var writer = new StreamWriter(GetAbsolutePath(_filename), false))
            {
                try
                {
                    writer.Write(JsonConvert.SerializeObject(CurrentConfig));
                }
                catch (Exception e)
                {
                    Logger.Fatal(e);
                    Logger.Fatal("Failed to save config!");
                    return false;
                }
            }

            return true;
        }

        public bool LoadOrCreateConfig()
        {
            Logger.Info("Loading config...");
            var path = GetAbsolutePath(_filename);
            if (!File.Exists(path))
            {
                Logger.Warn("Config does not exist! Creating default...");
                CurrentConfig = ServiceConfig.CreateDefault();
                SaveConfig();
            }
            else
            {
                try
                {
                    using (var reader = new StreamReader(path))
                        CurrentConfig = JsonConvert.DeserializeObject<ServiceConfig>(reader.ReadToEnd());

                    _deviceConfigs = Directory.EnumerateFiles(GetAbsolutePath(@"Plugins\Devices\"), "*.json")
                        .Select(f =>
                        {
                            using (var reader = new StreamReader(f))
                            {
                                return (Name: Path.GetFileNameWithoutExtension(f),
                                        Config: JsonConvert.DeserializeObject<DeviceConfig>(reader.ReadToEnd()));
                            }
                        })
                        .Where(x => x != default)
                        .ToDictionary(x => x.Name, x => x.Config);
                }
                catch (Exception e)
                {
                    Logger.Fatal(e);
                }

                if (CurrentConfig == null)
                {
                    Logger.Fatal("Failed to load the config!");
                    return false;
                }
            }

            return true;
        }

        private string GetAbsolutePath(string relativePath)
            => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), relativePath);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Config Manager...");

            CurrentConfig?.IpcServer?.Dispose();
            CurrentConfig = null;
        }

        public void Accept(ICacheCollector collector)
        {
            foreach (var (ports, config) in CurrentConfig.PortConfigs)
            {
                foreach (var port in ports)
                {
                    collector.StorePortConfig(port, config);

                    if (_deviceConfigs.ContainsKey(config.DeviceType))
                        collector.StoreDeviceConfig(port, _deviceConfigs[config.DeviceType]);
                    else if (!string.Equals(config.DeviceType, "Default", StringComparison.OrdinalIgnoreCase))
                        Logger.Warn("Unable to find device with name \"{0}\"!", config.DeviceType);
                }
            }

            foreach (var (sensors, config) in CurrentConfig.SensorConfigs)
                foreach (var sensor in sensors)
                    collector.StoreSensorConfig(sensor, config);
        }
    }
}
