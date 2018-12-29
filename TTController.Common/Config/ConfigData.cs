using System.Collections.Generic;

namespace TTController.Common.Config
{
    public class ConfigData
    {
        public List<ProfileData> Profiles { set; get; } = new List<ProfileData>();
        public IDictionary<PortIdentifier, PortConfigData> PortConfig { set; get; } = new Dictionary<PortIdentifier, PortConfigData>();

        public int TemperatureTimerInterval { set; get; } = 250;
        public int DeviceSpeedTimerInterval { set; get; } = 2500;
        public int DeviceRgbTimerInterval { set; get; } = (int) (1000.0 / 60.0);
        
        public static ConfigData CreateDefault()
        {
            return new ConfigData();
        }
    }
}
