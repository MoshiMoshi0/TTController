using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public class PortIdentifierConverter : JsonConverter<PortIdentifier>
    {
        public override void WriteJson(JsonWriter writer, PortIdentifier value, JsonSerializer serializer)
        {
            var token = JToken.FromObject(value);
            var array = new JArray(((JObject) token).Properties().Select(p => p.Value));
            writer.WriteRawValue(JsonConvert.SerializeObject(array, Formatting.None));
        }

        public override PortIdentifier ReadJson(JsonReader reader, Type objectType, PortIdentifier existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return new PortIdentifier(array[0].Value<int>(), array[1].Value<int>(), (byte)array[2].Value<int>());
        }
    }
}
