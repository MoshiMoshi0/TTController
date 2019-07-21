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
        private static RzChromaBroadcastManager _manager;
        private static int _instances;

        private readonly LedColor[] _colors;

        private bool _connected;

        public override string EffectType => "PerLed";
        public override bool IsEnabled(ICacheProvider cache) => base.IsEnabled(cache) && _connected;

        public RazerConnectEffect(RazerConnectEffectConfig config) : base(config)
        {
            if (_instances++ == 0)
            {
                _manager = new RzChromaBroadcastManager();
            }

            if(_manager != null)
            {
                _manager.ColorChanged += OnColorUpdate;
                _manager.ConnectionChanged += OnConnectionUpdate;
            }
        }

        private void OnColorUpdate(object sender, RzBroadcastColorChangedEventArgs e)
        {
            for (var i = 0; i < e.Colors.Length; i++)
            {
                _colors[i].R = (byte)((e.Colors[i] >> 0) & 0xff);
                _colors[i].G = (byte)((e.Colors[i] >> 8) & 0xff);
                _colors[i].B = (byte)((e.Colors[i] >> 16) & 0xff);
            }
        }

        private void OnConnectionUpdate(object sender, RzBroadcastConnectionChangedEventArgs e)
        {
            _connected = e.Connected;
        }

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            return ports.ToDictionary(p => p, _ => _colors.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_manager != null)
            {
                _manager.ColorChanged -= OnColorUpdate;
                _manager.ConnectionChanged -= OnConnectionUpdate;
            }

            if(--_instances == 0)
            {
                _manager?.Dispose();
                _manager = null;
            }
        }
    }
}
