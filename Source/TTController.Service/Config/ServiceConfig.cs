using System.Collections.Generic;
using System.ComponentModel;
using LibreHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Service.Config
{
    public class ServiceConfig
    {
        public List<ProfileConfig> Profiles { get; private set; } = new List<ProfileConfig>();
        public List<ComputerStateProfileConfig> ComputerStateProfiles { get; private set; } = new List<ComputerStateProfileConfig>();
        public List<(List<PortIdentifier> Ports, PortConfig Config)> PortConfigs { get; private set; } = new List<(List<PortIdentifier> Ports, PortConfig Config)>();
        public List<(List<Identifier> Sensors, SensorConfig Config)> SensorConfigs { get; private set; } = new List<(List<Identifier> Sensors, SensorConfig Config)>();

        [DefaultValue(null)] public IIpcServer IpcServer { get; private set; } = null;
        [DefaultValue(false)] public bool IpcServerEnabled { get; private set; } = false;

        [DefaultValue(true)] public bool CpuSensorsEnabled { get; private set; } = true;
        [DefaultValue(true)] public bool GpuSensorsEnabled { get; private set; } = true;
        [DefaultValue(false)] public bool StorageSensorsEnabled { get; private set; } = false;
        [DefaultValue(false)] public bool MotherboardSensorsEnabled { get; private set; } = false;
        [DefaultValue(false)] public bool MemorySensorsEnabled { get; private set; } = false;
        [DefaultValue(false)] public bool NetworkSensorsEnabled { get; private set; } = false;
        [DefaultValue(false)] public bool ControllerSensorsEnabled { get; private set; } = false;

        [DefaultValue(250)] public int SensorTimerInterval { get; private set; } = 250;
        [DefaultValue(2500)] public int DeviceSpeedTimerInterval { get; private set; } = 2500;
        [DefaultValue(32)] public int DeviceRgbTimerInterval { get; private set; } = 32;
        [DefaultValue(0)] public int IpcClientTimerInterval { get; private set; } = 0;
        [DefaultValue(5000)] public int DebugTimerInterval { get; private set; } = 5000;

        public static ServiceConfig CreateDefault()
        {
            return new ServiceConfig();
        }
    }
}
