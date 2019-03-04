using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public abstract class AbstractDictionaryConverter<TKey, TValue> : JsonConverter<IDictionary<TKey, TValue>>
    {
        public override void WriteJson(JsonWriter writer, IDictionary<TKey, TValue> value, JsonSerializer serializer)
        {
            var o = new JObject();
            foreach (var (k, v) in value)
                o.Add(WriteKey(k), WriteValue(v));
            o.WriteTo(writer);
        }

        public override IDictionary<TKey, TValue> ReadJson(JsonReader reader, Type objectType, IDictionary<TKey, TValue> existingValue, bool hasExistingValue,
            JsonSerializer serializer) =>
            JObject.Load(reader).Properties().ToDictionary(ReadKey, ReadValue);

        protected abstract TKey ReadKey(JProperty property);
        protected abstract TValue ReadValue(JProperty property);
        protected abstract string WriteKey(TKey key);
        protected abstract JToken WriteValue(TValue value);
    }
}
