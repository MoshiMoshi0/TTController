using System.Collections.Generic;
using System.ComponentModel;
using LibreHardwareMonitor.Hardware;
using TTController.Common;

namespace TTController.Service.Config.Data
{
    public class ConfigData
    {
        public List<ProfileData> Profiles { get; private set; } = new List<ProfileData>();
        public List<ComputerStateProfileData> ComputerStateProfiles { get; private set; } = new List<ComputerStateProfileData>();
        public List<(List<PortIdentifier> Ports, PortConfig Config)> PortConfigs { get; private set; } = new List<(List<PortIdentifier> Ports, PortConfig Config)>();
        public List<(List<Identifier> Sensors, SensorConfig Config)> SensorConfigs { get; private set; } = new List<(List<Identifier> Sensors, SensorConfig Config)>();

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
