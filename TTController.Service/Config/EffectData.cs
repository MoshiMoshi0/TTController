using System;
using TTController.Service.Rgb;

namespace TTController.Service.Config
{
    public class EffectData
    {
        public Type Type { get; private set; }
        public EffectConfigBase Config { get; private set; }

        public EffectData(Type type, EffectConfigBase config)
        {
            Type = type;
            Config = config;
        }
    }
}
