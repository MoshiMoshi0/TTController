using System;
using TTController.Common;

namespace TTController.Service.Config.Data
{
    public class SpeedControllerData
    {
        public Type Type { get; private set; }
        public SpeedControllerConfigBase Config { get; private set; }

        public SpeedControllerData(Type type, SpeedControllerConfigBase config)
        {
            Type = type;
            Config = config;
        }
    }
}
