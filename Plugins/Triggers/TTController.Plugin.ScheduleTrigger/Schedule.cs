using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach (var entry in Entries)
                if (time >= entry.Start && time <= entry.End)
                    return true;

            return false;
        }
    }
}