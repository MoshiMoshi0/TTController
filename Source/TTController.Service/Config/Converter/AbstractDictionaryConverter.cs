using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public abstract class AbstractDictionaryConverter<TKey, TValue> : JsonConverter<IDictionary<TKey, TValue>>
    {
        public override void WriteJson(JsonWriter writer, IDictionary<TKey, TValue> value, JsonSerializer serializer)
        {
            var o = new JArray();
            foreach (var (k, v) in value)
            {
                o.Add(new JObject
                {
                    { KeyName(), WriteKey(k) },
                    { ValueName(), WriteValue(v) }
                });
            }
            o.WriteTo(writer);
        }

        public override IDictionary<TKey, TValue> ReadJson(JsonReader reader, Type objectType, IDictionary<TKey, TValue> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (JObject o in JArray.Load(reader))
            {
                if (!o.ContainsKey(KeyName()))
                    throw new JsonReaderException($"Missing required property: \"{KeyName()}\"");
                if (!o.ContainsKey(ValueName()))
                    throw new JsonReaderException($"Missing required property: \"{ValueName()}\"");

                var key = ReadKey(o[KeyName()] as JProperty);
                var value = ReadValue(o[ValueName()] as JProperty);
                result.Add(key, value);
            }

            return result;
        }

        protected abstract TKey ReadKey(JProperty property);
        protected abstract TValue ReadValue(JProperty property);
        protected abstract JToken WriteKey(TKey key);
        protected abstract JToken WriteValue(TValue value);
        protected abstract string KeyName();
        protected abstract string ValueName();
    }
}
