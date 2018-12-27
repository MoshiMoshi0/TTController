using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;
using TTController.Common.Config;

namespace TTController.Service.Rgb
{
    public class StaticByLedEffect : EffectBase
    {
        private bool _needsUpdate;
        private readonly List<LedColor> _colors;

        public override byte EffectByte => (byte) EffectType.ByLed;
        public override bool NeedsUpdate() => _needsUpdate;

        public StaticByLedEffect(dynamic config) : base((object)config)
        {
            _needsUpdate = true;

            _colors = JsonConvert.DeserializeObject<List<LedColor>>((config["Colors"] as JToken).ToString());
        }

        public override IList<IEnumerable<LedColor>> GenerateColors(IEnumerable<PortConfigData> ports)
        {
            _needsUpdate = false;
            return ports.Select(p => _colors.AsEnumerable()).ToList();
        }


    }
}
