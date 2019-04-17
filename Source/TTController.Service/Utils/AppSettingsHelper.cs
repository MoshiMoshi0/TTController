using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Service.Utils
{
    public static class AppSettingsHelper
    {
        public static string ReadValue(string key)
        {
            var configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configCollection = configManager.AppSettings.Settings;

            return configCollection[key]?.Value;
        }

        public static T ReadValue<T>(string key, T defaultValue = default)
        {
            try
            {
                return (T) Convert.ChangeType(ReadValue(key), typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void WriteValue(string key, string value)
        {
            var configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configCollection = configManager.AppSettings.Settings;

            configCollection.Add(key, value);
            configManager.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configManager.AppSettings.SectionInformation.Name);
        }

        public static void WriteValue<T>(string key, T value)
            => WriteValue(key, value.ToString());
    }
}
