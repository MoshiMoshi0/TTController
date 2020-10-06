using System.Collections.Generic;
using System.ComponentModel;
using LibreHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Service.Config
{
    public class ServiceConfig
    {
        public List<ProfileConfig> Profiles { get; internal set; } = new List<ProfileConfig>();
        public List<ComputerStateProfileConfig> ComputerStateProfiles { get; internal set; } = new List<ComputerStateProfileConfig>();
        public List<(List<PortIdentifier> Ports, PortConfig Config)> PortConfigs { get; internal set; } = new List<(List<PortIdentifier> Ports, PortConfig Config)>();
        public List<(List<Identifier> Sensors, SensorConfig Config)> SensorConfigs { get; internal set; } = new List<(List<Identifier> Sensors, SensorConfig Config)>();

        [DefaultValue(null)] public IIpcServer IpcServer { get; internal set; } = null;
        [DefaultValue(false)] public bool IpcServerEnabled { get; internal set; } = false;

        [DefaultValue(true)] public bool CpuSensorsEnabled { get; internal set; } = true;
        [DefaultValue(true)] public bool GpuSensorsEnabled { get; internal set; } = true;
        [DefaultValue(false)] public bool StorageSensorsEnabled { get; internal set; } = false;
        [DefaultValue(false)] public bool MotherboardSensorsEnabled { get; internal set; } = false;
        [DefaultValue(false)] public bool MemorySensorsEnabled { get; internal set; } = false;
        [DefaultValue(false)] public bool NetworkSensorsEnabled { get; internal set; } = false;
        [DefaultValue(false)] public bool ControllerSensorsEnabled { get; internal set; } = false;

        [DefaultValue(250)] public int SensorTimerInterval { get; internal set; } = 250;
        [DefaultValue(2500)] public int DeviceSpeedTimerInterval { get; internal set; } = 2500;
        [DefaultValue(32)] public int DeviceRgbTimerInterval { get; internal set; } = 32;
        [DefaultValue(0)] public int IpcClientTimerInterval { get; internal set; } = 0;
        [DefaultValue(5000)] public int DebugTimerInterval { get; internal set; } = 5000;

        public static ServiceConfig CreateDefault()
        {
            return new ServiceConfig();
        }
    }
}
