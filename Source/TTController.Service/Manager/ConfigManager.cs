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
    public sealed class ConfigManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _filename;

        public ConfigData CurrentConfig { get; private set; }

        public ConfigManager(string filename)
        {
            Logger.Info("Creating Config Manager...");
            _filename = filename;

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Culture = CultureInfo.InvariantCulture,
                ContractResolver = new ContractResolver()
            };
            jsonSettings.Error += (sender, args) =>
            {
                if (args.CurrentObject == args.ErrorContext.OriginalObject)
                    Logger.Fatal(args.ErrorContext.Error.Message);
            };

            var converters = typeof(JsonConverter).FindInAssemblies()
                    .Where(t => t.Namespace?.StartsWith("TTController") ?? false)
                    .Where(t => !t.IsGenericType && !t.IsAbstract)
                    .Select(t => (JsonConverter)Activator.CreateInstance(t));

            jsonSettings.Converters.Add(new StringEnumConverter());
            foreach (var converter in converters)
                jsonSettings.Converters.Add(converter);

            JsonConvert.DefaultSettings = () => jsonSettings;
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

                    Logger.Fatal("Failed to save config!");
                    return false;
                }
            }

            return true;
        }

        public bool LoadOrCreateConfig()
        {
            Logger.Info("Loading config...");
            var path = GetConfigAbsolutePath();
            if (!File.Exists(path))
            {
                Logger.Warn("Config does not exist! Creating default...");
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
                    Logger.Fatal("Failed to load the config!");
                    return false;
                }
            }

            return true;
        }

        private string GetConfigAbsolutePath()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return Path.Combine(directory, _filename);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Config Manager...");

            CurrentConfig = null;
        }
    }
}
