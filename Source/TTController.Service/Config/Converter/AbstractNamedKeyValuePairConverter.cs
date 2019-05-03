using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Service.Config.Converter
{
    public abstract class AbstractNamedKeyValuePairConverter<TKey, TValue> : JsonConverter<KeyValuePair<TKey, TValue>>
    {
        public override void WriteJson(JsonWriter writer, KeyValuePair<TKey, TValue> value, JsonSerializer serializer)
        {
            var o = new JObject
            {
                { KeyName(), JsonConvert.SerializeObject(value.Key) },
                { ValueName(), JsonConvert.SerializeObject(value.Value) }
            };
            o.WriteTo(writer);
        }

        public override KeyValuePair<TKey, TValue> ReadJson(JsonReader reader, Type objectType, KeyValuePair<TKey, TValue> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            var key = (TKey) JsonConvert.DeserializeObject(o[KeyName()].ToString(), typeof(TKey));
            var value = (TValue) JsonConvert.DeserializeObject(o[ValueName()].ToString(), typeof(TValue));

            return new KeyValuePair<TKey, TValue>(key, value);
        }

        protected abstract string KeyName();
        protected abstract string ValueName();
    }
}
