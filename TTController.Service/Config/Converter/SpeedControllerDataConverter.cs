using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Service.Speed;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public class SpeedControllerDataConverter : JsonConverter<SpeedControllerData>
    {
        public override void WriteJson(JsonWriter writer, SpeedControllerData value, JsonSerializer serializer)
        {
            var o = new JObject
            {
                {"Type", JToken.FromObject(value.Type.Name)},
                {"Config", JToken.FromObject(value.Config)}
            };
            o.WriteTo(writer);
        }

        public override SpeedControllerData ReadJson(JsonReader reader, Type objectType, SpeedControllerData existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var o = JObject.ReadFrom(reader);
            var speedControllerTypeName = (o.First() as JProperty).Value.ToString();
            var speedControllerType = TypeUtils.FindInAssemblies<ISpeedControllerBase>()
                .First(t => string.CompareOrdinal(t.Name, speedControllerTypeName) == 0);

            var configType = TypeUtils.FindInAssemblies<SpeedControllerConfigBase>()
                .First(t => string.CompareOrdinal(t.Name, $"{speedControllerTypeName}Config") == 0);

            var json = (o.Last() as JProperty).Value.ToString();
            var config = (SpeedControllerConfigBase) JsonConvert.DeserializeObject(json, configType);
            return new SpeedControllerData(speedControllerType, config);
        }
    }
}
