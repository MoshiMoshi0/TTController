using NLog;
using System;
using System.Runtime.InteropServices;

namespace TTController.Plugin.RazerConnectEffect
{
    public class RzChromaBroadcastManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly RzBroadcastCallback _callback;
        private readonly int[] _colors;

        public bool Initialized { get; private set; }

        public event EventHandler<RzBroadcastColorChangedEventArgs> ColorChanged;
        public event EventHandler<RzBroadcastConnectionChangedEventArgs> ConnectionChanged;

        public RzChromaBroadcastManager()
        {
            if (!RzChromaBroadcastNative.Load())
                return;

            var guid = Guid.Parse("b0ecdaf9-26b2-d33f-f046-1c44ce64eb58");
            if (RzChromaBroadcastNative.Init(guid) != RzResult.SUCCESS)
                return;

            _callback = new RzBroadcastCallback(BroadcastCallback);
            if (RzChromaBroadcastNative.RegisterEventNotification(_callback) != RzResult.SUCCESS)
                return;

            _colors = new int[RzChromaBroadcastNative.BroadcastColorCount];
            Initialized = true;
        }

        private int BroadcastCallback(int message, IntPtr data)
        {
            if (message == 1)
            {
                if (data != IntPtr.Zero)
                {
                    Marshal.Copy(data, _colors, 0, RzChromaBroadcastNative.BroadcastColorCount);

                    Logger.Trace("Razer broadcast colors updated");
                    ColorChanged?.Invoke(this, new RzBroadcastColorChangedEventArgs(_colors));
                }
            }
            else if (message == 2)
            {
                var connected = data.ToInt32() == 1;
                Logger.Trace("Razer broadcast connection changed: {0}", connected);
                ConnectionChanged?.Invoke(this, new RzBroadcastConnectionChangedEventArgs(connected));
            }

            return 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            _ = RzChromaBroadcastNative.UnregisterEventNotification();
            _ = RzChromaBroadcastNative.UnInit();
            RzChromaBroadcastNative.UnLoad();

            Initialized = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class RzBroadcastColorChangedEventArgs
    {
        public int[] Colors { get; }

        public RzBroadcastColorChangedEventArgs(int[] colors)
        {
            Colors = colors;
        }
    }

    public class RzBroadcastConnectionChangedEventArgs
    {
        public bool Connected { get; }

        public RzBroadcastConnectionChangedEventArgs(bool connected)
        {
            Connected = connected;
        }
    }
}
