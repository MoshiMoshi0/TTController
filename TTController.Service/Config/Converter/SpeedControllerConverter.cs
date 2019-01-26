using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;
using TTController.Service.Config.Data;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public class SpeedControllerConverter : JsonConverter<ISpeedControllerBase>
    {
        public override void WriteJson(JsonWriter writer, ISpeedControllerBase value, JsonSerializer serializer)
        {
            var speedControllerType = value.GetType();
            var speedControllerName = speedControllerType.Name;
            var speedControllerConfig = speedControllerType.GetProperty("Config").GetValue(value, null);

            var o = new JObject
            {
                {"Type", JToken.FromObject(speedControllerName)},
                {"Config", JToken.FromObject(speedControllerConfig ?? new object())}
            };

            o.WriteTo(writer);
        }

        public override ISpeedControllerBase ReadJson(JsonReader reader, Type objectType, ISpeedControllerBase existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var o = JObject.ReadFrom(reader);
            var speedControllerTypeName = (o.First() as JProperty).Value.ToString();
            var speedControllerType = typeof(ISpeedControllerBase).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, speedControllerTypeName) == 0);

            var configType = typeof(SpeedControllerConfigBase).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, $"{speedControllerTypeName}Config") == 0);

            var json = (o.Last() as JProperty).Value.ToString();
            var config = (SpeedControllerConfigBase)JsonConvert.DeserializeObject(json, configType);
            return (ISpeedControllerBase)Activator.CreateInstance(speedControllerType, new object[] { config });
        }
    }
}
