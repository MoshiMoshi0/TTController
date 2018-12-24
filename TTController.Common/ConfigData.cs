using System.Collections.Generic;

namespace TTController.Common
{
    public class ConfigData
    {
        public List<ProfileData> Profiles { protected set; get; } = new List<ProfileData>();
        public int TemperatureTimerInterval { protected set; get; } = 250;
        public int DeviceSpeedTimerInterval { protected set; get; } = 2500;
        public int DeviceRgbTimerInterval { protected set; get; } = (int)(1000 / 60.0);
        
        public static ConfigData CreateDefault()
        {
            return new ConfigData();
        }
    }
}
