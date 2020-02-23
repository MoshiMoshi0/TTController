using System;
using System.ComponentModel;
using System.Threading;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.ScheduleTrigger
{
    public enum ScheduleScope
    {
        Minute,
        Hour,
        Day,
        Week
    }

    public class ScheduleTriggerConfig : TriggerConfigBase
    {
        [DefaultValue(ScheduleScope.Day)] public ScheduleScope Scope { get; internal set; } = ScheduleScope.Day;
        [DefaultValue(true)] public bool Value { get; internal set; } = true;
        [DefaultValue(null)] public TimeSpan? UpdateInterval { get; internal set; } = null;
        [DefaultValue(null)] public Schedule Schedule { get; internal set; } = null;
    }

    public class ScheduleTrigger : TriggerBase<ScheduleTriggerConfig>
    {
        private readonly Timer _timer;
        private volatile bool _state;

        public ScheduleTrigger(ScheduleTriggerConfig config) : base(config)
        {
            TimeSpan period;
            if (!Config.UpdateInterval.HasValue)
            {
                switch (Config.Scope)
                {
                    case ScheduleScope.Minute: period = TimeSpan.FromSeconds(1); break;
                    case ScheduleScope.Hour: period = TimeSpan.FromMinutes(1); break;
                    case ScheduleScope.Week: period = TimeSpan.FromMinutes(15); break;
                    default: period = TimeSpan.FromMinutes(1); break;
                }
            }
            else
            {
                period = Config.UpdateInterval.Value;
            }

            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, period);
        }

        public override bool Value(ICacheProvider cache) => _state;

        private void TimerCallback(object state)
        {
            var date = DateTime.Now;

            int day = 0, hour = 0, minute = 0, second = 0;
            if (Config.Scope >= ScheduleScope.Minute) second = date.Second;
            if (Config.Scope >= ScheduleScope.Hour) minute = date.Minute;
            if (Config.Scope >= ScheduleScope.Day) hour = date.Hour;
            if (Config.Scope >= ScheduleScope.Week) day = (int)date.DayOfWeek;

            if (Config.Schedule?.Contains(new TimeSpan(day, hour, minute, second)) ?? false)
                _state = Config.Value;
            else
                _state = !Config.Value;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _timer.Dispose();
        }
    }
}
