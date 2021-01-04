using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using LibreHardwareMonitor.Hardware;
using NLog;
using TTController.Common;
using TTController.Common.Plugin;
using TTController.Service.Config;
using TTController.Service.Managers;
using TTController.Service.Utils;

namespace TTController.Service
{
    internal class TTService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private DeviceManager _deviceManager;
        private ConfigManager _configManager;
        private SensorManager _sensorManager;
        private TimerManager _timerManager;

        private PluginStore _pluginStore;
        private DataCache _cache;
        private ServiceConfig _config;

        private ServiceIpcClient _ipcClient;

        protected bool IsDisposed;

        public TTService()
        {
            ServiceName = TTInstaller.ServiceName;

            CanStop = true;
            CanShutdown = true;
            CanHandlePowerEvent = true;
            CanPauseAndContinue = false;
        }

        public bool Initialize()
        {
            Logger.Info($"{new string('=', 64)}");
            Logger.Info("Initializing service, version \"{0}\"", FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location)?.ProductVersion);
            PluginLoader.LoadAll(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

            var serializationContext = new TrackingSerializationContext(typeof(IPlugin), typeof(Identifier));
            _configManager = new ConfigManager("config.json", serializationContext);
            if (!_configManager.LoadOrCreateConfig())
                return false;

            _config = _configManager.CurrentConfig;
            _cache = new DataCache();
            _pluginStore = new PluginStore(serializationContext.Get<IPlugin>());

            _sensorManager = new SensorManager(_config);
            _deviceManager = new DeviceManager();

            _sensorManager.EnableSensors(_config.SensorConfigs.SelectMany(x => x.Sensors));
            foreach (var profile in _config.Profiles)
            {
                Logger.Info("Processing profile \"{0}\"", profile.Name);
                if (_pluginStore.Get(profile).Any())
                {
                    Logger.Fatal("Duplicate profile \"{0}\" found!", profile.Name);
                    return false;
                }

                foreach (var effect in profile.Effects)
                    _pluginStore.Assign(effect, profile);

                foreach (var speedController in profile.SpeedControllers)
                    _pluginStore.Assign(speedController, profile);

                foreach(var port in profile.Ports)
                {
                    _cache.StorePortConfig(port, PortConfig.Default);
                    _cache.StoreDeviceConfig(port, DeviceConfig.Default);

                    if (!_deviceManager.Controllers.SelectMany(c => c.Ports).Contains(port))
                        Logger.Warn("Could not find matching controller for port {0}!", port);
                }
            }

            _sensorManager.EnableSensors(serializationContext.Get<Identifier>());
            foreach (var sensor in _sensorManager.EnabledSensors)
                _cache.StoreSensorConfig(sensor, SensorConfig.Default);

            foreach (var controller in _deviceManager.Controllers)
            {
                foreach (var port in controller.Ports)
                {
                    _cache.StorePortConfig(port, PortConfig.Default);
                    _cache.StoreDeviceConfig(port, DeviceConfig.Default);
                }
            }

            _configManager.Accept(_cache.AsWriteOnly());

            if (_config.IpcServerEnabled && _config.IpcServer != null)
            {
                _ipcClient = new ServiceIpcClient(_config.Profiles.SelectMany(p => p.Ports), _cache.AsReadOnly());
                _pluginStore.Add(_ipcClient);
                _config.IpcServer.Register(_ipcClient); 

                foreach (var plugin in serializationContext.Get<IIpcClient>())
                    _config.IpcServer.Register(plugin);
                _config.IpcServer.Start();
            }

            ApplyComputerStateProfile(ComputerStateType.Boot);

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_config.SensorTimerInterval, SensorTimerCallback);
            _timerManager.RegisterTimer(_config.DeviceSpeedTimerInterval, DeviceSpeedTimerCallback);
            _timerManager.RegisterTimer(_config.DeviceRgbTimerInterval, DeviceRgbTimerCallback);

            if(LogManager.Configuration.LoggingRules.Any(r => r.IsLoggingEnabledForLevel(LogLevel.Debug)))
                _timerManager.RegisterTimer(_config.DebugTimerInterval, DebugTimerCallback);

            _timerManager.Start();

            Logger.Info("Initializing done!");
            Logger.Info($"{new string('=', 64)}");
            return true;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                if (!Initialize())
                    throw new Exception("Service failed to start!");

                IsDisposed = false;
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                ExitCode = 1;
                Stop();
                throw;
            }
        }

        protected override void OnStop()
        {
            Finalize();
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            Finalize();
            base.OnShutdown();
        }

        protected void OnSuspend()
        {
            Finalize(ComputerStateType.Suspend);
            base.OnStop();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            Logger.Debug("Power Event: {0}", powerStatus);

            switch (powerStatus)
            {
                case PowerBroadcastStatus.QuerySuspendFailed:
                    Logger.Warn("System failed to enter Suspend state!");
                    OnStart(null);
                    break;

                case PowerBroadcastStatus.ResumeAutomatic:
                    OnStart(null);
                    break;

                case PowerBroadcastStatus.ResumeCritical:
                case PowerBroadcastStatus.ResumeSuspend:
                    break;

                case PowerBroadcastStatus.QuerySuspend:
                case PowerBroadcastStatus.Suspend:
                    OnSuspend();
                    break;

                default:
                    break;
            }

            return base.OnPowerEvent(powerStatus);
        }

        public void Finalize(ComputerStateType state = ComputerStateType.Shutdown)
        {
            if (IsDisposed)
                return;

            Logger.Info($"{new string('=', 64)}");
            Logger.Info("Finalizing...");

            _timerManager?.Dispose();

            if(_deviceManager != null)
                ApplyComputerStateProfile(state);

            _sensorManager?.Dispose();
            _deviceManager?.Dispose();
            _configManager?.Dispose();

            _pluginStore?.Dispose();
            _cache?.Clear();

            _timerManager = null;
            _sensorManager = null;
            _deviceManager = null;
            _configManager = null;

            _pluginStore = null;
            _cache = null;

            Dispose();
            IsDisposed = true;

            Logger.Info("Finalizing done!");
            Logger.Info($"{new string('=', 64)}");
        }

        private void ApplyComputerStateProfile(ComputerStateType state)
        {
            if (state == ComputerStateType.Boot)
            {
                const string key = "boot-profile-saved";
                if (AppSettingsHelper.ReadValue<bool>(key))
                    return;

                AppSettingsHelper.WriteValue(key, true);
            }

            Logger.Info("Applying computer state profile: {0}", state);
            foreach (var profile in _config.ComputerStateProfiles.Where(p => p.StateType == state))
            {
                var speed = profile.Speed;
                var colorMap = profile.Effect?.GetColors(profile.Ports, _cache);

                if (speed == null && colorMap == null)
                    continue;

                foreach (var port in profile.Ports)
                {
                    var controller = _deviceManager.GetController(port);
                    if (controller == null)
                        continue;

                    lock (controller)
                    {
                        var isDirty = false;
                        if (speed != null)
                            isDirty |= controller.SetSpeed(port.Id, profile.Speed.Value);

                        var colors = colorMap?[port];
                        if (colors != null)
                        {
                            var config = _cache.GetPortConfig(port);
                            foreach (var modifier in config.ColorModifiers)
                                modifier.Apply(ref colors, port, _cache.AsReadOnly());

                            isDirty |= controller.SetRgb(port.Id, profile.Effect.EffectType, colors);
                        }

                        if (state == ComputerStateType.Boot && isDirty)
                            controller.SaveProfile();
                    }
                }
            }
        }

        #region Timer Callbacks
        private bool SensorTimerCallback()
        {
            _sensorManager.Update();
            _sensorManager.Accept(_cache.AsWriteOnly());
            return true;
        }

        private bool DeviceSpeedTimerCallback()
        {
            var criticalState = _sensorManager.EnabledSensors.Any(s => {
                var value = _cache.GetSensorValue(s);
                if (float.IsNaN(value))
                    return false;

                var config = _cache.GetSensorConfig(s);
                if (config == null || config.CriticalValue == null)
                    return false;

                return value > config.CriticalValue;
            });

            foreach (var profile in _config.Profiles)
            {
                foreach (var port in profile.Ports)
                {
                    var controller = _deviceManager.GetController(port);
                    if (controller == null)
                        continue;

                    lock (controller)
                    {
                        var data = controller?.GetPortData(port.Id);
                        _cache.StorePortData(port, data);
                    }
                }

                IDictionary<PortIdentifier, byte> speedMap;
                try
                {
                    if (criticalState)
                    {
                        speedMap = profile.Ports.ToDictionary(p => p, _ => (byte)100);
                    }
                    else
                    {
                        var speedController = _pluginStore
                            .Get<ISpeedControllerBase>(profile)
                            .FirstOrDefault(c => c.IsEnabled(_cache.AsReadOnly()));
                        if (speedController == null)
                            continue;

                        speedMap = speedController.GetSpeeds(profile.Ports, _cache.AsReadOnly());
                    }
                }
                catch(Exception e)
                {
                    Logger.Fatal(e);
                    speedMap = profile.Ports.ToDictionary(p => p, _ => (byte)100);
                }

                if (speedMap == null)
                    continue;

                foreach (var (port, speed) in speedMap)
                {
                    if (!_cache.GetPortConfig(port).IgnoreSpeedCache && speed == _cache.GetPortSpeed(port))
                        continue;

                    var controller = _deviceManager.GetController(port);
                    if (controller == null)
                        continue;

                    lock (controller)
                    {
                        controller.SetSpeed(port.Id, speed);
                        _cache.StorePortSpeed(port, speed);
                    }
                }
            }

            return true;
        }

        public bool DeviceRgbTimerCallback()
        {
            foreach (var profile in _config.Profiles)
            {
                IDictionary<PortIdentifier, List<LedColor>> colorMap;
                string effectType;
                try
                {
                    var effect = _pluginStore
                       .Get<IEffectBase>(profile)
                       .FirstOrDefault(e => e.IsEnabled(_cache.AsReadOnly()));
                    if (effect == null)
                        continue;

                    effect.Update(_cache.AsReadOnly());
                    colorMap = effect.GetColors(profile.Ports, _cache.AsReadOnly());
                    effectType = effect.EffectType;
                }
                catch (Exception e)
                {
                    Logger.Fatal(e);
                    colorMap = profile.Ports.ToDictionary(p => p, _ => new List<LedColor>() { new LedColor(255, 0, 0) });
                    effectType = "PerLed";
                }

                if (colorMap == null || effectType == null)
                    continue;

                foreach (var (port, _) in colorMap)
                {
                    var controller = _deviceManager.GetController(port);
                    if (controller == null)
                        continue;

                    var colors = colorMap[port];
                    if (colors == null)
                        continue;

                    var config = _cache.GetPortConfig(port);
                    foreach (var modifier in config.ColorModifiers)
                        modifier.Apply(ref colors, port, _cache.AsReadOnly());

                    if (!config.IgnoreColorCache && colors.ContentsEqual(_cache.GetPortColors(port)))
                        continue;

                    lock (controller)
                    {
                        controller.SetRgb(port.Id, effectType, colors);
                        _cache.StorePortColors(port, colors);
                    }
                }
            }

            return true;
        }

        public bool DebugTimerCallback()
        {
            foreach (var profile in _config.Profiles)
            {
                foreach (var port in profile.Ports)
                {
                    var data = _cache.GetPortData(port);
                    if (data == null)
                        continue;

                    Logger.Debug("Port {0} data: {1}", port, data);
                }
            }

            foreach (var identifier in _sensorManager.EnabledSensors)
            {
                var value = _sensorManager.GetSensorValue(identifier);
                if (float.IsNaN(value))
                    continue;
                Logger.Debug("Sensor \"{0}\" value: {1}", identifier, value);
            }

            return true;
        }
        #endregion
    }
}
