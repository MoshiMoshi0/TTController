using System.Collections.Generic;
using OpenHardwareMonitor.Hardware;
using TTController.Common;

namespace TTController.Service.Config
{
    public class ConfigData
    {
        public List<ProfileData> Profiles { get; private set; } = new List<ProfileData>();
        public List<StateChangeProfileData> StateChangeProfiles { get; private set; } = new List<StateChangeProfileData>();
        public IDictionary<PortIdentifier, PortConfigData> PortConfig { get; private set; } = new Dictionary<PortIdentifier, PortConfigData>();
        public IDictionary<Identifier, int> CriticalTemperature { get; private set; } = new Dictionary<Identifier, int>();

        public int TemperatureTimerInterval { get; private set; } = 250;
        public int DeviceSpeedTimerInterval { get; private set; } = 2500;
        public int DeviceRgbTimerInterval { get; private set; } = (int) (1000.0 / 30.0);
        
        public static ConfigData CreateDefault()
        {
            return new ConfigData();
        }
    }
}
