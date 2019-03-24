using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TTController.Service.Config.Converter
{
    public abstract class ObjectToArrayConverter<T> : JsonConverter<T>
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            var properties = FilterProperties(value);
            var array = new JArray(properties.Select(p => p.GetValue(value)));
            writer.WriteRawValue(JsonConvert.SerializeObject(array, Formatting.None));
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return (T)Activator.CreateInstance(typeof(T), CreateConstructorArgs(array));
        }

        protected virtual IEnumerable<PropertyInfo> FilterProperties(T value) => value.GetType().GetProperties();
        protected abstract object[] CreateConstructorArgs(JArray array);
    }
}
