using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Common;

namespace TTController.Plugin.FlowEffect
{
    public class FlowEffectConfig : EffectConfigBase
    {
        public float FillStep { get; set; }
        public float HueStep { get; set; }
        public float Saturation { get; set; }
        public float Brightness { get; set; }
    }

    public class FlowEffect : EffectBase<FlowEffectConfig>
    {
        private float _currentHue;
        private float _lastHue;
        private float _fill;


        public FlowEffect(FlowEffectConfig config) : base(config)
        {
            _lastHue = 0;
            _currentHue = Config.HueStep;
        }

        public override byte EffectByte => (byte) EffectType.ByLed;

        public override IDictionary<PortIdentifier, List<LedColor>> GenerateColors(List<PortIdentifier> ports, ICacheProvider cache)
        {
            _fill += Config.FillStep;
            if (_fill >= 1)
            {
                _fill = 0;
                _lastHue = _currentHue;
                _currentHue = (_currentHue + Config.HueStep) % 360;
            }
            
            var result = new Dictionary<PortIdentifier, List<LedColor>>();
            foreach (var port in ports)
            {
                var config = cache.GetPortConfig(port);
                if(config == null)
                    continue;

                var lastColor = LedColor.FromHsv(_lastHue, Config.Saturation, Config.Brightness);
                var currentColor = LedColor.FromHsv(_currentHue, Config.Saturation, Config.Brightness);

                var colors = Enumerable.Range(0, config.LedCount).Select(x => lastColor).ToList();
                for (var i = 0; i < (int) Math.Round(config.LedCount * _fill); i++)
                {
                    colors[i] = currentColor;
                }

                result.Add(port, colors);
            }

            return result;
        }
    }
}
