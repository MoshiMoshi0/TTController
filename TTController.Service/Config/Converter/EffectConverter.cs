using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;
using TTController.Service.Config.Data;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public class EffectConverter : JsonConverter<IEffectBase>
    {
        public override void WriteJson(JsonWriter writer, IEffectBase value, JsonSerializer serializer)
        {
            var effectType = value.GetType();
            var effectName = effectType.Name;
            var effectConfig = effectType.GetProperty("Config").GetValue(value, null);

            var o = new JObject
            {
                {"Type", JToken.FromObject(effectName)},
                {"Config", JToken.FromObject(effectConfig ?? new object())}
            };

            o.WriteTo(writer);
        }

        public override IEffectBase ReadJson(JsonReader reader, Type objectType, IEffectBase existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.ReadFrom(reader);
            var effectTypeName = (o.First() as JProperty).Value.ToString();
            var effectType = typeof(IEffectBase).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, effectTypeName) == 0);

            var configType = typeof(EffectConfigBase).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, $"{effectTypeName}Config") == 0);

            var json = (o.Last() as JProperty).Value.ToString();
            var config = (EffectConfigBase) JsonConvert.DeserializeObject(json, configType);
            return (IEffectBase)Activator.CreateInstance(effectType, new object[] {config});
        }
    }
}
