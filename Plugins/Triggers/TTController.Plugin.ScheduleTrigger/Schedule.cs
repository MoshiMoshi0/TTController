using System;
using System.Collections.Generic;
using System.Linq;

namespace TTController.Plugin.ScheduleTrigger
{
    public class Schedule
    {
        public IReadOnlyList<(TimeSpan Start, TimeSpan End)> Entries { get; }

        public Schedule(IEnumerable<(TimeSpan Start, TimeSpan End)> entries)
        {
            Entries = entries.ToList();
        }

        public bool Contains(TimeSpan time)
        {
            foreach (var (start, end) in Entries)
                if (time >= start && time <= end)
                    return true;

            return false;
        }
    }
}