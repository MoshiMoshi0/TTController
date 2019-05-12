using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenHardwareMonitor.Hardware;

namespace TTController.Service.Config.Converter
{
    public class IdentifierConverter : JsonConverter<Identifier>
    {
        public override void WriteJson(JsonWriter writer, Identifier value, JsonSerializer serializer) =>
            writer.WriteValue(value.ToString());

        public override Identifier ReadJson(JsonReader reader, Type objectType, Identifier existingValue, bool hasExistingValue,
            JsonSerializer serializer) =>
            new Identifier(JToken.ReadFrom(reader).Value<string>().Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries));
    }
}
