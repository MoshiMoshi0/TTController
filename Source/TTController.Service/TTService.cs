using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
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
            Logger.Info("Initializing...");
            PluginLoader.LoadAll(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));

            _configManager = new ConfigManager("config.json");
            if (!_configManager.LoadOrCreateConfig())
                return false;

            _config = _configManager.CurrentConfig;
            _cache = new DataCache();
            _pluginStore = new PluginStore();

            _sensorManager = new SensorManager(_config);
            _deviceManager = new DeviceManager();

            _sensorManager.EnableSensors(_config.SensorConfigs.SelectMany(x => x.Sensors));
            foreach (var profile in _config.Profiles)
            {
                if (_pluginStore.Get(profile).Any())
                {
                    Logger.Fatal("Duplicate profile \"{0}\" found!", profile.Name);
                    return false;
                }

                foreach (var effect in profile.Effects)
                {
                    _pluginStore.Add(profile, effect);
                    _sensorManager.EnableSensors(effect.UsedSensors);
                }

                foreach (var speedController in profile.SpeedControllers)
                {
                    _pluginStore.Add(profile, speedController);
                    _sensorManager.EnableSensors(speedController.UsedSensors);
                }

                profile.Ports.RemoveAll(p =>
                {
                    var portExists = _deviceManager.Controllers.SelectMany(c => c.Ports).Contains(p);
                    if (!portExists)
                        Logger.Warn("Removing invalid port: {0}", p);

                    return !portExists;
                });
            }

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

            ApplyComputerStateProfile(ComputerStateType.Boot);

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_config.SensorTimerInterval, SensorTimerCallback);
            _timerManager.RegisterTimer(_config.DeviceSpeedTimerInterval, DeviceSpeedTimerCallback);
            _timerManager.RegisterTimer(_config.DeviceRgbTimerInterval, DeviceRgbTimerCallback);
            if(LogManager.Configuration.LoggingRules.Any(r => r.IsLoggingEnabledForLevel(LogLevel.Debug)))
                _timerManager.RegisterTimer(_config.LoggingTimerInterval, LoggingTimerCallback);

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
            lock (_deviceManager)
            {
                var dirtyControllers = new HashSet<IControllerProxy>();
                foreach (var profile in _config.ComputerStateProfiles.Where(p => p.StateType == state))
                {
                    foreach (var port in profile.Ports)
                    {
                        var controller = _deviceManager.GetController(port);
                        if (controller == null)
                            continue;

                        if(profile.Speed.HasValue)
                            controller.SetSpeed(port.Id, profile.Speed.Value);

                        var effectByte = controller.GetEffectByte(profile.EffectType);
                        if (effectByte.HasValue && profile.Color != null)
                            controller.SetRgb(port.Id, effectByte.Value, profile.Color.Get(_cache.GetDeviceConfig(port).LedCount));

                        if (state == ComputerStateType.Boot && (profile.Speed.HasValue || effectByte.HasValue))
                            dirtyControllers.Add(controller);
                    }
                }

                foreach(var controller in dirtyControllers)
                    controller.SaveProfile();
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
                lock (_deviceManager)
                {
                    foreach (var port in profile.Ports)
                    {
                        var controller = _deviceManager.GetController(port);
                        var data = controller?.GetPortData(port.Id);
                        _cache.StorePortData(port, data);
                    }
                }

                IDictionary<PortIdentifier, byte> speedMap;
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

                    try
                    {
                        speedMap = speedController.GenerateSpeeds(profile.Ports, _cache.AsReadOnly());
                    }
                    catch(Exception e)
                    {
                        Logger.Fatal("{0} failed with {1}", speedController.GetType().Name, e);
                        speedMap = profile.Ports.ToDictionary(p => p, _ => (byte)100);
                    }
                }

                if (speedMap == null)
                    continue;

                lock (_deviceManager)
                {
                    foreach (var (port, speed) in speedMap)
                    {
                        if (speed == _cache.GetPortSpeed(port))
                            continue;

                        var controller = _deviceManager.GetController(port);
                        if (controller == null)
                            continue;

                        _cache.StorePortSpeed(port, speed);
                        controller.SetSpeed(port.Id, speed);
                    }
                }
            }

            return true;
        }

        public bool DeviceRgbTimerCallback()
        {
            void ApplyConfig(IDictionary<PortIdentifier, List<LedColor>> colorMap)
            {
                foreach (var port in colorMap.Keys.ToList())
                {
                    var config = _cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    var colors = colorMap[port];
                    var ledCount = _cache.GetDeviceConfig(port).LedCount;
                    var zones = _cache.GetDeviceConfig(port).Zones;

                    switch (config.LedCountHandling)
                    {
                        case LedCountHandling.Lerp:
                            {
                                if (ledCount == colors.Count)
                                    break;

                                var newColors = new List<LedColor>();
                                var gradient = new LedColorGradient(colors, ledCount - 1);

                                for (var i = 0; i < ledCount; i++)
                                    newColors.Add(gradient.GetColor(i));

                                colors = newColors;
                                break;
                            }
                        case LedCountHandling.Nearest:
                            {
                                if (ledCount == colors.Count)
                                    break;

                                var newColors = new List<LedColor>();
                                for (var i = 0; i < ledCount; i++) {
                                    var idx = (int)Math.Round((i / (ledCount - 1d)) * (colors.Count - 1d));
                                    newColors.Add(colors[idx]);
                                }

                                colors = newColors;
                                break;
                            }
                        case LedCountHandling.Wrap:
                            if (colors.Count <= ledCount)
                                break;

                            var wrapCount = (int)Math.Floor(colors.Count / (double)ledCount);
                            var startOffset = (wrapCount - 1) * ledCount;
                            var remainder = colors.Count - wrapCount * ledCount;

                            colors = colors.Skip(colors.Count - remainder)
                                .Concat(colors.Skip(startOffset + remainder).Take(ledCount - remainder))
                                .ToList();
                            break;
                        case LedCountHandling.Trim:
                            if (ledCount < colors.Count)
                                colors.RemoveRange(ledCount, colors.Count - ledCount);
                            break;
                        case LedCountHandling.Copy:
                            while (ledCount > colors.Count)
                                colors.AddRange(colors.Take(ledCount - colors.Count).ToList());
                            break;
                        case LedCountHandling.DoNothing:
                        default:
                            break;
                    }

                    if (config.LedRotation != null || config.LedReverse != null)
                    {
                        var offset = 0;
                        var newColors = new List<LedColor>();
                        for (int i = 0; i < zones.Length; i++)
                        {
                            var zoneColors = colors.Skip(offset).Take(zones[i]);

                            if (i < config.LedRotation?.Length && config.LedRotation[i] > 0)
                                zoneColors = zoneColors.RotateLeft(config.LedRotation[i]);
                            if (i < config.LedReverse?.Length && config.LedReverse[i])
                                zoneColors = zoneColors.Reverse();

                            offset += zones[i];
                            newColors.AddRange(zoneColors);
                        }

                        if (newColors.Count < colors.Count)
                            newColors.AddRange(colors.Skip(offset));

                        colors = newColors;
                    }

                    colorMap[port] = colors;
                }
            }

            foreach (var profile in _config.Profiles)
            {
                var effect = _pluginStore
                    .Get<IEffectBase>(profile)
                    .FirstOrDefault(e => e.IsEnabled(_cache.AsReadOnly()));
                if (effect == null)
                    continue;

                IDictionary<PortIdentifier, List<LedColor>> colorMap;
                string effectType;

                try
                {
                    colorMap = effect.GenerateColors(profile.Ports, _cache.AsReadOnly());
                    effectType = effect.EffectType;
                }
                catch (Exception e)
                {
                    Logger.Fatal("{0} failed with {1}", effect.GetType().Name, e);
                    colorMap = profile.Ports.ToDictionary(p => p, _ => new List<LedColor>() { new LedColor(255, 0, 0) });
                    effectType = "Full";
                }

                if (colorMap == null)
                    continue;

                ApplyConfig(colorMap);

                lock (_deviceManager)
                {
                    foreach (var (port, colors) in colorMap)
                    {
                        if (colors == null)
                            continue;

                        if (colors.ContentsEqual(_cache.GetPortColors(port)))
                            continue;

                        var controller = _deviceManager.GetController(port);
                        var effectByte = controller?.GetEffectByte(effectType);
                        if (effectByte == null)
                            continue;

                        _cache.StorePortColors(port, colors);
                        controller.SetRgb(port.Id, effectByte.Value, colors);
                    }
                }
            }

            return true;
        }

        public bool LoggingTimerCallback()
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

            lock (_sensorManager)
            {
                foreach (var identifier in _sensorManager.EnabledSensors)
                {
                    var value = _sensorManager.GetSensorValue(identifier);
                    if (float.IsNaN(value))
                        continue;
                    Logger.Debug("Sensor \"{0}\" value: {1}", identifier, value);
                }
            }

            return true;
        }
        #endregion
    }
}
