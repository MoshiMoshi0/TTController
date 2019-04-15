using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace TTController.Service.Manager
{
    public sealed class TimerManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<Timer> _timers;

        public TimerManager()
        {
            Logger.Info("Creating Timer Manager...");
            _timers = new List<Timer>();
        }

        public void RegisterTimer(int interval, Func<bool> callback)
        {
            if (interval <= 0)
                return;

            var timer = new Timer(interval)
            {
                UseThreadSpinWait = false
            };

            timer.Elapsed += (sender, args) =>
            {
                if (!(sender is Timer _this))
                    return;

                if (!callback())
                    _this.Stop();
            };

            _timers.Add(timer);
        }

        public void Start()
        {
            Logger.Info("Starting {0} timers...", _timers.Count);
            foreach (var timer in _timers)
                timer.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing TimerManager...");
            Logger.Info("Stopping {0} timers...", _timers.Count);
            foreach (var timer in _timers)
                timer.Stop();

            _timers.Clear();
        }

        #region Timer
        /// <summary>
        /// Hight precision non overlapping timer
        /// Came from 
        /// https://stackoverflow.com/a/41697139/548894
        /// </summary>
        /// <remarks>
        /// This implementation guaranteed that Elapsed events 
        /// are not overlapped with different threads. 
        /// Which is important, because a state of the event handler attached to  Elapsed,
        /// may be left unprotected of multi threaded access
        /// </remarks>
        public class Timer
        {
            /// <summary>
            /// Tick time length in [ms]
            /// </summary>
            public static readonly double TickLength = 1000f / Stopwatch.Frequency;

            /// <summary>
            /// Tick frequency
            /// </summary>
            public static readonly double Frequency = Stopwatch.Frequency;

            /// <summary>
            /// True if the system/operating system supports HighResolution timer
            /// </summary>
            public static readonly bool IsHighResolution = Stopwatch.IsHighResolution;

            /// <summary>
            /// Invoked when the timer is elapsed
            /// </summary>
            public event EventHandler<TimerElapsedEventArgs> Elapsed;

            /// <summary>
            /// The interval of timer ticks [ms]
            /// </summary>
            private volatile float _interval;

            /// <summary>
            /// The timer is running
            /// </summary>
            private volatile bool _isRunning;

            /// <summary>
            ///  Execution thread
            /// </summary>
            private Thread _thread;

            /// <summary>
            /// Creates a timer with 1 [ms] interval
            /// </summary>
            public Timer() : this(1f)
            {
            }

            /// <summary>
            /// Creates timer with interval in [ms]
            /// </summary>
            /// <param name="interval">Interval time in [ms]</param>
            public Timer(float interval)
            {
                Interval = interval;
            }

            /// <summary>
            /// The interval of a timer in [ms]
            /// </summary>
            public float Interval
            {
                get => _interval;
                set
                {
                    if (value < 0f || Single.IsNaN(value))
                    {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }
                    _interval = value;
                }
            }

            /// <summary>
            /// True when timer is running
            /// </summary>
            public bool IsRunning => _isRunning;

            /// <summary>
            /// If true, sets the execution thread to ThreadPriority.Highest
            /// (works after the next Start())
            /// </summary>
            /// <remarks>
            /// It might help in some cases and get things worse in others. 
            /// It suggested that you do some studies if you apply
            /// </remarks>
            public bool UseHighPriorityThread { get; set; } = false;

            /// <summary>
            /// If true, uses <see cref="Thread.SpinWait(int)"/> to
            /// make the timer delay as close to 0 [ms] as possible
            /// </summary>
            /// <remarks>
            /// Trades precision for cpu usage
            /// </remarks>
            public bool UseThreadSpinWait { get; set; } = true;

            /// <summary>
            /// Starts the timer
            /// </summary>
            public void Start()
            {
                if (_isRunning) return;

                _isRunning = true;
                _thread = new Thread(ExecuteTimer)
                {
                    IsBackground = true,
                };

                if (UseHighPriorityThread)
                {
                    _thread.Priority = ThreadPriority.Highest;
                }
                _thread.Start();
            }

            /// <summary>
            /// Stops the timer
            /// </summary>
            /// <remarks>
            /// This function is waiting an executing thread (which do  to stop and join.
            /// </remarks>
            public void Stop(bool joinThread = true)
            {
                _isRunning = false;

                // Even if _thread.Join may take time it is guaranteed that 
                // Elapsed event is never called overlapped with different threads
                if (joinThread && Thread.CurrentThread != _thread)
                {
                    _thread.Join();
                }
            }

            private void ExecuteTimer()
            {
                float nextTrigger = 0f;

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (_isRunning)
                {
                    nextTrigger += _interval;
                    double elapsed;

                    while (true)
                    {
                        elapsed = ElapsedHiRes(stopwatch);
                        double diff = nextTrigger - elapsed;
                        if (diff <= 0f)
                            break;

                        if (UseThreadSpinWait && diff < 1f)
                            Thread.SpinWait(10);
                        else if (UseThreadSpinWait && diff < 5f)
                            Thread.SpinWait(100);
                        else if (diff < 15f)
                            Thread.Sleep(1);
                        else
                            Thread.Sleep(10);

                        if (!_isRunning)
                            return;
                    }

                    double delay = elapsed - nextTrigger;
                    Elapsed?.Invoke(this, new TimerElapsedEventArgs(delay));

                    if (!_isRunning)
                        return;

                    // restarting the timer in every hour to prevent precision problems
                    if (stopwatch.Elapsed.TotalHours >= 1d)
                    {
                        stopwatch.Restart();
                        nextTrigger = 0f;
                    }
                }

                stopwatch.Stop();
            }

            private static double ElapsedHiRes(Stopwatch stopwatch)
            {
                return stopwatch.ElapsedTicks * TickLength;
            }
        }

        public class TimerElapsedEventArgs : EventArgs
        {
            /// <summary>/// Real timer delay in [ms]/// </summary>
            public double Delay { get; }

            internal TimerElapsedEventArgs(double delay)
            {
                Delay = delay;
            }
        }
        #endregion
    }
}
