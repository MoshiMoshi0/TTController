using System;
using System.Collections.Generic;
using System.Timers;

namespace TTController.Service.Manager
{
    public class TimerManager : IDisposable
    {
        private readonly List<Timer> _timers;

        public TimerManager()
        {
            _timers = new List<Timer>();
        }

        public void RegisterTimer(int interval, Func<bool> callback)
        {
            var timer = new Timer()
            {
                AutoReset = false,
                Interval = interval
            };

            timer.Elapsed += (sender, args) =>
            {
                if (!(sender is Timer _this))
                    return;

                if (callback())
                    _this.Start();
            };

            _timers.Add(timer);
        }

        public void Start()
        {
            foreach (var timer in _timers)
                timer.Start();
        }
        
        public void Dispose()
        {
            foreach (var timer in _timers)
                timer.Close();
        }
    }
}
