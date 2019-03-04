using System;
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

    public class IdentifierToIntDictionaryConverter : AbstractDictionaryConverter<Identifier, int>
    {
        protected override Identifier ReadKey(JProperty property) =>
            new Identifier(property.Name.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries));

        protected override int ReadValue(JProperty property) =>
            property.Value.ToObject<int>();

        protected override string WriteKey(Identifier key) =>
            key.ToString();

        protected override JToken WriteValue(int value) =>
            JToken.FromObject(value);
    }
}
