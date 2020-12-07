using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.RazerConnectEffect
{
    public enum RazerConnectLayer
    {
        Base,
        Custom,
        Both
    }

    public class RazerConnectEffectConfig : EffectConfigBase
    {
        [DefaultValue(RazerConnectLayer.Custom)] public RazerConnectLayer Layer { get; internal set; } = RazerConnectLayer.Custom;
    }

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
                _manager = new RzChromaBroadcastManager();

            _colors = new LedColor[RzChromaBroadcastNative.BroadcastColorCount];
            if (_manager?.Initialized == true)
            {
                _manager.ColorChanged += OnColorUpdate;
                _manager.ConnectionChanged += OnConnectionUpdate;
            }
        }

        private void OnColorUpdate(object sender, RzBroadcastColorChangedEventArgs e)
        {
            Logger.Trace("Razer broadcast colors updated");
            for (var i = 0; i < _colors.Length; i++)
                _colors[i] = LedColor.Unpack(e.Colors[i]);
        }

        private void OnConnectionUpdate(object sender, RzBroadcastConnectionChangedEventArgs e)
        {
            Logger.Trace("Razer broadcast connection changed: {0}", e.Connected);
            _connected = e.Connected;
        }

        protected override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
            => ports.ToDictionary(p => p, _ => GenerateColors(0, cache));

        protected override List<LedColor> GenerateColors(int count, ICacheProvider cache)
        {
            if (Config.Layer == RazerConnectLayer.Base)
                return _colors.Take(1).ToList();
            else if (Config.Layer == RazerConnectLayer.Custom)
                return _colors.Skip(1).ToList();
            else if (Config.Layer == RazerConnectLayer.Both)
                return _colors.ToList();

            return null;
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
