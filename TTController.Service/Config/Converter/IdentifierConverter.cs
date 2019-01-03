using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Config.Converter
{
    public class IdentifierConverter : JsonConverter<Identifier>
    {
        public override void WriteJson(JsonWriter writer, Identifier value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Identifier ReadJson(JsonReader reader, Type objectType, Identifier existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.ReadFrom(reader);
            return new Identifier(o.Value<string>().Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries));
        }
    }

    public class IdentifierDictionaryConverter<TValue> : JsonConverter<IDictionary<Identifier, TValue>>
    {
        public override void WriteJson(JsonWriter writer, IDictionary<Identifier, TValue> value, JsonSerializer serializer)
        {
            var o = new JObject();
            foreach (var kvp in value)
                o.Add(kvp.Key.ToString(), JToken.FromObject(kvp.Value));
            o.WriteTo(writer);
        }

        public override IDictionary<Identifier, TValue> ReadJson(JsonReader reader, Type objectType, IDictionary<Identifier, TValue> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            var result = new Dictionary<Identifier, TValue>();
            foreach (var prop in o.Properties())
                result.Add(new Identifier(prop.Name.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries)),
                    prop.Value.ToObject<TValue>());
            return result;
        }
    }
}
