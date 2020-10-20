using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converters
{
    public abstract class AbstractPluginConverter<TPlugin, TConfig> : JsonConverter<TPlugin>
    {
        public override TPlugin ReadJson(JsonReader reader, Type objectType, TPlugin existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var o = JToken.ReadFrom(reader) as JObject;

            var typeProperty = o.GetValue("Type");
            if (typeProperty == null)
                throw new JsonReaderException("Missing required property: \"Type\"");

            var configProperty = o.GetValue("Config");

            var pluginTypeName = typeProperty.ToString();
            var configTypeName = $"{pluginTypeName}Config";

            Type pluginType;
            try
            {
                pluginType = typeof(TPlugin).FindImplementations()
                    .First(t => string.CompareOrdinal(t.Name, pluginTypeName) == 0);
            }
            catch
            {
                throw new JsonReaderException($"Invalid plugin name \"{pluginTypeName}\"");
            }

            var configType = pluginType.BaseType.GetGenericArguments().First();
            var configJson = configProperty?.ToString();
            var config = string.IsNullOrEmpty(configJson)
                ? (TConfig)Activator.CreateInstance(configType)
                : (TConfig)JsonConvert.DeserializeObject(configJson, configType);

            var result = (TPlugin)Activator.CreateInstance(pluginType, config);
            var contract = serializer.ContractResolver.ResolveContract(pluginType);
            foreach (var callback in contract.OnDeserializedCallbacks)
                callback(result, serializer.Context);

            return result;
        }

        public override void WriteJson(JsonWriter writer, TPlugin value, JsonSerializer serializer)
        {
            var pluginType = value.GetType();
            var config = pluginType.GetProperty("Config")?.GetValue(value, null);

            var o = new JObject
            {
                {"Type", JToken.FromObject(pluginType.Name)}
            };

            if (config != null)
                o.Add("Config", JToken.FromObject(config));

            o.WriteTo(writer);
        }
    }
}
