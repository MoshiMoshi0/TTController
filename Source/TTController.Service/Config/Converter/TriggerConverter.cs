using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public class TriggerConverter : JsonConverter<ITriggerBase>
    {
        public override void WriteJson(JsonWriter writer, ITriggerBase value, JsonSerializer serializer)
        {
            var triggerType = value.GetType();
            var triggerName = triggerType.Name;
            var triggerConfig = triggerType.GetProperty("Config").GetValue(value, null);
            
            var o = new JObject {{triggerName, JToken.FromObject(triggerConfig ?? new object()) }};
            o.WriteTo(writer);
        }

        public override ITriggerBase ReadJson(JsonReader reader, Type objectType, ITriggerBase existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.ReadFrom(reader).First() as JProperty;
            var triggerTypeName = o.Name;
            var triggerType = typeof(ITriggerBase).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, triggerTypeName) == 0);
            
            var configType = typeof(TriggerConfigBase).FindInAssemblies()
                .First(t => string.CompareOrdinal(t.Name, $"{triggerTypeName}Config") == 0);

            var config = (TriggerConfigBase)JsonConvert.DeserializeObject(o.Value.ToString(), configType);
            return (ITriggerBase)Activator.CreateInstance(triggerType, new object[] { config });
        }
    }
}
