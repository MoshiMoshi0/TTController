using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Plugin.ScheduleTrigger
{
    class ScheduleConverter : JsonConverter<Schedule>
    {
        private readonly string Separator = " -> ";

        public override void WriteJson(JsonWriter writer, Schedule value, JsonSerializer serializer)
        {
            var array = new JArray();
            foreach (var entry in value.Entries)
                array.Add($"{entry.Start:c}{Separator}{entry.End:c}");

            serializer.Serialize(writer, array);
        }

        public override Schedule ReadJson(JsonReader reader, Type objectType, Schedule existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var entries = new List<(TimeSpan Start, TimeSpan End)>();

            var array = JArray.Load(reader);
            foreach(var s in array.Values<string>())
            {
                var parts = s.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                if (TimeSpan.TryParse(parts[0], out var start) && TimeSpan.TryParse(parts[1], out var end))
                    entries.Add((start, end));
            }

            return new Schedule(entries);
        }
    }
}
