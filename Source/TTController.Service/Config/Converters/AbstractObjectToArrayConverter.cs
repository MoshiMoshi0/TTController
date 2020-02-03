using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TTController.Service.Config.Converters
{
    public abstract class AbstractObjectToArrayConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var properties = FilterProperties((T)value);
            var array = new JArray(properties.Select(p => p.GetValue((T)value)));
            writer.WriteRawValue(JsonConvert.SerializeObject(array, Formatting.None));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return (T)Activator.CreateInstance(typeof(T), CreateConstructorArgs(array));
        }

        public override bool CanConvert(Type objectType)
            => objectType == typeof(T) || Nullable.GetUnderlyingType(objectType) == typeof(T);

        protected virtual IEnumerable<PropertyInfo> FilterProperties(T value) => value.GetType().GetProperties();
        protected abstract object[] CreateConstructorArgs(JArray array);
    }
}
