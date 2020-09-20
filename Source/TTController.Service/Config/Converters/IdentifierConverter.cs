using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LibreHardwareMonitor.Hardware;

namespace TTController.Service.Config.Converters
{
    public class IdentifierConverter : JsonConverter<Identifier>
    {
        public override void WriteJson(JsonWriter writer, Identifier value, JsonSerializer serializer) =>
            writer.WriteValue(value.ToString());

        public override Identifier ReadJson(JsonReader reader, Type objectType, Identifier existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var result = new Identifier(JToken.ReadFrom(reader).Value<string>().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));

            var contract = serializer.ContractResolver.ResolveContract(objectType); 
            foreach (var callback in contract.OnDeserializedCallbacks)
                callback(result, serializer.Context);

            return result;
        }
    }
}
