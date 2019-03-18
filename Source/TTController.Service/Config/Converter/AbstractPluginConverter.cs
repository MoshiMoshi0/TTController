using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public abstract class AbstractPluginConverter<TPlugin, TConfig> : JsonConverter<TPlugin>
    {
        public override TPlugin ReadJson(JsonReader reader, Type objectType, TPlugin existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var o = JToken.ReadFrom(reader);

            var typeProperty = (o.First() as JProperty);
            var configProperty = (o.Last() as JProperty);

            var pluginTypeName = typeProperty.Value.ToString();
            var configTypeName = $"{pluginTypeName}Config";

            var pluginType = typeof(TPlugin).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, pluginTypeName) == 0);

            var configType = typeof(TConfig).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, configTypeName) == 0);

            var json = configProperty.Value.ToString();
            var config = (TConfig)JsonConvert.DeserializeObject(json, configType);
            return (TPlugin)Activator.CreateInstance(pluginType, config);
        }

        public override void WriteJson(JsonWriter writer, TPlugin value, JsonSerializer serializer)
        {
            var pluginType = value.GetType();
            var config = pluginType.GetProperty("Config")?.GetValue(value, null) ?? new object();

            var o = new JObject
            {
                {"Type", JToken.FromObject(pluginType.Name)},
                {"Config", JToken.FromObject(config)}
            };

            o.WriteTo(writer);
        }
    }
}
