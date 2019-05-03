using System.Collections.Generic;
using System.ComponentModel;
using OpenHardwareMonitor.Hardware;
using TTController.Common;

namespace TTController.Service.Config.Data
{
    public class ConfigData
    {
        public List<ProfileData> Profiles { get; private set; } = new List<ProfileData>();
        public List<ComputerStateProfileData> ComputerStateProfiles { get; private set; } = new List<ComputerStateProfileData>();
        public Dictionary<List<PortIdentifier>, PortConfig> PortConfigs { get; private set; } = new Dictionary<List<PortIdentifier>, PortConfig>();
        public IDictionary<Identifier, int> CriticalTemperature { get; private set; } = new Dictionary<Identifier, int>();

        [DefaultValue(250)] public int SensorTimerInterval { get; private set; } = 250;
        [DefaultValue(2500)] public int DeviceSpeedTimerInterval { get; private set; } = 2500;
        [DefaultValue(32)] public int DeviceRgbTimerInterval { get; private set; } = 32;
        [DefaultValue(5000)] public int LoggingTimerInterval { get; private set; } = 5000;

        public static ConfigData CreateDefault()
        {
            return new ConfigData();
        }
    }
}
