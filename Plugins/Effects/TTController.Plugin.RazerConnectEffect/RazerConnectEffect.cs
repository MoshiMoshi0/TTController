using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RazerConnectEffect
{
    public class RazerConnectEffectConfig : EffectConfigBase { }

    public class RazerConnectEffect : EffectBase<RazerConnectEffectConfig>
    {
        private readonly int BroadcastColorCount = 5;

        private readonly LedColor[] _colors;
        private readonly int[] _packedColors;

        private bool _connected;

        public override string EffectType => "ByLed";
        public override bool IsEnabled(ICacheProvider cache) => base.IsEnabled(cache) && _connected;

        public RazerConnectEffect(RazerConnectEffectConfig config) : base(config)
        {
            _connected = false;
            _colors = new LedColor[BroadcastColorCount];
            _packedColors = new int[BroadcastColorCount];

            RzChromaBroadcastNative.Load();
            var init = RzChromaBroadcastNative.Init(Guid.Parse("b0ecdaf9-26b2-d33f-f046-1c44ce64eb58"));
            if (init == RzResult.SUCCESS)
            {
                var register = RzChromaBroadcastNative.RegisterEventNotification(BroadcastCallback);
                if (register == RzResult.SUCCESS)
                    _connected = true;
            }
        }

        private int BroadcastCallback(int message, IntPtr data)
        {
            if (message == 1)
            {
                if (data != IntPtr.Zero)
                {
                    Marshal.Copy(data, _packedColors, 0, BroadcastColorCount);

                    for (var i = 0; i < BroadcastColorCount; i++)
                    {
                        _colors[i].R = (byte)((_packedColors[i] >> 0) & 0xf);
                        _colors[i].G = (byte)((_packedColors[i] >> 8) & 0xff);
                        _colors[i].B = (byte)((_packedColors[i] >> 16) & 0xff);
                    }
                }
            }
            else if (message == 2)
            {
                if (data.ToInt32() == 1)
                    _connected = true;
                else if (data.ToInt32() == 2)
                    _connected = false;
            }

            return 0;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, _ => _colors.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            var unRegister = RzChromaBroadcastNative.UnregisterEventNotification();
            var unInit = RzChromaBroadcastNative.UnInit();
            RzChromaBroadcastNative.UnLoad();
        }
    }
}
