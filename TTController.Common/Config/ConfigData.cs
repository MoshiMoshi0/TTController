using System.Collections.Generic;

namespace TTController.Common.Config
{
    public class ConfigData
    {
        public List<ProfileData> Profiles { protected set; get; } = new List<ProfileData>();
        public List<PortConfigData> PortConfig { protected set; get; } = new List<PortConfigData>();

        public int TemperatureTimerInterval { protected set; get; } = 250;
        public int DeviceSpeedTimerInterval { protected set; get; } = 2500;
        public int DeviceRgbTimerInterval { protected set; get; } = (int)(1000 / 60.0);
        
        public static ConfigData CreateDefault()
        {
            var result = new ConfigData();
            result.Profiles.Add(new ProfileData("test"));
            return result;
        }
    }
}
