using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public abstract class ObjectToArrayConverter<T> : JsonConverter<T> where T : new()
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            var token = JToken.FromObject(value);
            var properties = FilterProperties(((JObject)token).Properties());
            var array = new JArray(properties.Select(p => p.Value));
            writer.WriteRawValue(JsonConvert.SerializeObject(array, Formatting.None));
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return (T) Activator.CreateInstance(typeof(T), CreateConstructorArgs(array));
        }

        protected virtual IEnumerable<JProperty> FilterProperties(IEnumerable<JProperty> properties) => properties;
        protected abstract object[] CreateConstructorArgs(JArray array);
    }
}
