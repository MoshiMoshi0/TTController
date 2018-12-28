using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Service.Rgb;
using TTController.Service.Trigger;

namespace TTController.Service.Config.Converter
{
    public class TriggerConverter : JsonConverter<ITriggerBase>
    {
        public override void WriteJson(JsonWriter writer, ITriggerBase value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override ITriggerBase ReadJson(JsonReader reader, Type objectType, ITriggerBase existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var o = JObject.ReadFrom(reader);
            var pair = o.ToObject<Dictionary<string, dynamic>>().First();

            var type = Assembly
                .GetAssembly(typeof(ITriggerBase))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ITriggerBase).IsAssignableFrom(t))
                .FirstOrDefault(t => string.CompareOrdinal(t.Name, pair.Key) == 0);

            if (type == null)
                return null;

            if ((pair.Value as JObject).HasValues)
                return (ITriggerBase)Activator.CreateInstance(type, new object[] { pair.Value });
            return (ITriggerBase) Activator.CreateInstance(type);
        }
    }
}
