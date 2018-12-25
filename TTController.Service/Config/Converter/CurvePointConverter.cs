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
    public class CurvePointConverter : JsonConverter<CurvePoint>
    {
        public override void WriteJson(JsonWriter writer, CurvePoint value, JsonSerializer serializer)
        {
            var token = JToken.FromObject(value);
            var array = new JArray(((JObject)token).Properties().Select(p => p.Value));
            writer.WriteRawValue(JsonConvert.SerializeObject(array, Formatting.None));
        }

        public override CurvePoint ReadJson(JsonReader reader, Type objectType, CurvePoint existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return new CurvePoint(array[0].Value<int>(), array[1].Value<int>());
        }
    }
}
