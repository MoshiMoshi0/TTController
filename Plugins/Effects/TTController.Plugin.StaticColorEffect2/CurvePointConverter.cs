using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TTController.Plugin.StaticColorEffect2
{
    public class CurvePointConverter : JsonConverter<CurvePoint>
    {
        public override void WriteJson(JsonWriter writer, CurvePoint value, JsonSerializer serializer)
        {
            var array = new JArray { value.Value, value.Red, value.Green, value.Blue };
            writer.WriteRawValue(JsonConvert.SerializeObject(array, Formatting.None));
        }

        public override CurvePoint ReadJson(JsonReader reader, Type objectType, CurvePoint existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return new CurvePoint(array[0].Value<int>(), array[1].Value<byte>(), array[2].Value<byte>(), array[3].Value<byte>());
        }
    }
}