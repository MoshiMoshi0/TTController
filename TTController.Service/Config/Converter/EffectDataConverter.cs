using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Service.Rgb;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public class EffectDataConverter : JsonConverter<EffectData>
    {
        public override void WriteJson(JsonWriter writer, EffectData value, JsonSerializer serializer)
        {
            var o = new JObject
            {
                {"Type", JToken.FromObject(value.Type.Name)},
                {"Config", JToken.FromObject(value.Config)}
            };
            o.WriteTo(writer);
        }

        public override EffectData ReadJson(JsonReader reader, Type objectType, EffectData existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var o = JObject.ReadFrom(reader);
            var effectTypeName = (o.First() as JProperty).Value.ToString();
            var effectType = TypeUtils.FindInAssemblies<IEffectBase>()
                .First(t => string.CompareOrdinal(t.Name, effectTypeName) == 0);

            var configType = TypeUtils.FindInAssemblies<EffectConfigBase>()
                .First(t => string.CompareOrdinal(t.Name, $"{effectTypeName}Config") == 0);

            var json = (o.Last() as JProperty).Value.ToString();
            var config = (EffectConfigBase) JsonConvert.DeserializeObject(json, configType);
            return new EffectData(effectType, config);
        }
    }
}
