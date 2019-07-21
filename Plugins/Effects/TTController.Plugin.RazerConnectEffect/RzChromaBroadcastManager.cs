using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;

namespace TTController.Plugin.RazerConnectEffect
{
    public class RzChromaBroadcastManager : IDisposable
    {
        private readonly RzBroadcastCallback _callback;
        private readonly int[] _colors;

        public event EventHandler<RzBroadcastColorChangedEventArgs> ColorChanged;
        public event EventHandler<RzBroadcastConnectionChangedEventArgs> ConnectionChanged;

        public RzChromaBroadcastManager()
        {
            if (RzChromaBroadcastNative.Load())
            {
                var init = RzChromaBroadcastNative.Init(Guid.Parse("b0ecdaf9-26b2-d33f-f046-1c44ce64eb58"));
                if (init == RzResult.SUCCESS)
                {
                    _callback = new RzBroadcastCallback(BroadcastCallback);

                    var register = RzChromaBroadcastNative.RegisterEventNotification(_callback);
                    if (register == RzResult.SUCCESS)
                    {
                        _colors = new int[RzChromaBroadcastNative.BroadcastColorCount];
                    }
                }
            }
        }

        private int BroadcastCallback(int message, IntPtr data)
        {
            if (message == 1)
            {
                if (data != IntPtr.Zero)
                {
                    Marshal.Copy(data, _colors, 0, RzChromaBroadcastNative.BroadcastColorCount);
                    ColorChanged?.Invoke(this, new RzBroadcastColorChangedEventArgs(_colors));
                }
            }
            else if (message == 2)
            {
                ConnectionChanged?.Invoke(this, new RzBroadcastConnectionChangedEventArgs(data.ToInt32() == 1));
            }

            return 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            var unRegister = RzChromaBroadcastNative.UnregisterEventNotification();
            var unInit = RzChromaBroadcastNative.UnInit();
            RzChromaBroadcastNative.UnLoad();
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
