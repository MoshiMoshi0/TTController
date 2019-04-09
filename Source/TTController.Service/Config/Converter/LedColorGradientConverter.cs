using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public class LedColorGradientConverter : JsonConverter<LedColorGradient>
    {
        public override void WriteJson(JsonWriter writer, LedColorGradient value, JsonSerializer serializer) =>
            writer.WriteRawValue(JsonConvert.SerializeObject(value.Points));

        public override LedColorGradient ReadJson(JsonReader reader, Type objectType, LedColorGradient existingValue, bool hasExistingValue,
            JsonSerializer serializer) =>
            new LedColorGradient(JArray.Load(reader).ToObject<List<LedColorGradientPoint>>());
    }
}
