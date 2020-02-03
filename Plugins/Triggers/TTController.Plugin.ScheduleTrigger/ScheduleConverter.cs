using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TTController.Plugin.ScheduleTrigger
{
    public class ScheduleConverter : JsonConverter<Schedule>
    {
        private readonly string Separator = " -> ";
        private readonly string[] Formats = new string[] { @"d\.hh\:mm", @"hh\:mm", @"ss" };

        public override void WriteJson(JsonWriter writer, Schedule value, JsonSerializer serializer)
        {
            var array = new JArray();
            foreach (var (Start, End) in value.Entries)
            {
                var format = Formats[0];
                if (Start.TotalMinutes < 1 && End.TotalMinutes < 1)
                    format = Formats[2];
                else if (Start.TotalDays < 1 && End.TotalDays < 1)
                    format = Formats[1];

                var start = Start.ToString(format);
                var end = End.ToString(format);
                array.Add($"{start}{Separator}{end}");
            }

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

                if (TimeSpan.TryParseExact(parts[0], Formats, null, out var start)
                    && TimeSpan.TryParseExact(parts[1], Formats, null, out var end))
                {
                    if (start < TimeSpan.Zero || end < TimeSpan.Zero)
                        throw new JsonReaderException($"Invalid negative time: \"{s}\"");
                    if (start >= end)
                        throw new JsonReaderException($"Start time must be before End time: \"{s}\"");
                    if (start.Days > 7 || end.Days > 7)
                        throw new JsonReaderException($"Invalid day number: \"{s}\"");

                    entries.Add((start, end));
                }
            }

            return new Schedule(entries);
        }
    }
}
