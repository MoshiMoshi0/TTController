using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using TTController.Common;
using TTController.Service.Config.Data;
using TTController.Service.Hardware.Temperature;
using TTController.Service.Manager;
using TTController.Service.Utils;

namespace TTController.Service
{
    internal class TTService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private DeviceManager _deviceManager;
        private ConfigManager _configManager;
        private SensorManager _sensorManager;
        private TemperatureManager _temperatureManager;
        private TimerManager _timerManager;
        private EffectManager _effectManager;
        private SpeedControllerManager _speedControllerManager;
        private DataCache _cache;

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
            var pluginAssemblies = Directory.GetFiles($@"{AppDomain.CurrentDomain.BaseDirectory}\Plugins", "*.dll", SearchOption.AllDirectories)
                .Where(f => AppDomain.CurrentDomain.GetAssemblies().All(a => a.Location != f))
                .TrySelect(Assembly.LoadFile, _ => { })
                .ToList();

            AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
                pluginAssemblies.Find(a => string.CompareOrdinal(a.FullName, args.Name) == 0);

            Logger.Info("Loading plugins...");
            foreach (var assembly in pluginAssemblies)
                Logger.Info("Loading assembly: {0} [{1}]", assembly.GetName().Name, assembly.GetName().Version);

            const string key = "config-file";
            if (string.IsNullOrEmpty(AppSettingsHelper.ReadValue(key)))
                AppSettingsHelper.WriteValue(key, "config.json");

            _configManager = new ConfigManager(AppSettingsHelper.ReadValue(key));
            if (!_configManager.LoadOrCreateConfig())
                return false;

            _cache = new DataCache();
            _sensorManager = new SensorManager();

            var alpha = Math.Exp(-_configManager.CurrentConfig.TemperatureTimerInterval / (double)_configManager.CurrentConfig.DeviceSpeedTimerInterval);
            var providerFactory = new MovingAverageTemperatureProviderFactory(alpha);
            _temperatureManager = new TemperatureManager(_sensorManager.TemperatureSensors.ToList(), providerFactory);

            _effectManager = new EffectManager();
            _speedControllerManager = new SpeedControllerManager();
            _deviceManager = new DeviceManager();
            _deviceManager.Accept(_cache.AsWriteOnly());

            Logger.Info("Applying config...");
            _configManager.Accept(_cache.AsWriteOnly());
            foreach (var profile in _configManager.CurrentConfig.Profiles)
            {
                foreach (var effect in profile.Effects)
                    _effectManager.Add(profile.Guid, effect);

                foreach (var speedController in profile.SpeedControllers)
                    _speedControllerManager.Add(profile.Guid, speedController);

                _temperatureManager.EnableSensors(_speedControllerManager.GetSpeedControllers(profile.Guid)?.SelectMany(c => c.UsedSensors));
                _temperatureManager.EnableSensors(_effectManager.GetEffects(profile.Guid)?.SelectMany(e => e.UsedSensors));
            }

            ApplyComputerStateProfile(ComputerStateType.Boot);

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_configManager.CurrentConfig.TemperatureTimerInterval, TemperatureTimerCallback);
            _timerManager.RegisterTimer(_configManager.CurrentConfig.DeviceSpeedTimerInterval, DeviceSpeedTimerCallback);
            _timerManager.RegisterTimer(_configManager.CurrentConfig.DeviceRgbTimerInterval, DeviceRgbTimerCallback);
            if(LogManager.Configuration.LoggingRules.Any(r => r.IsLoggingEnabledForLevel(LogLevel.Debug)))
                _timerManager.RegisterTimer(_configManager.CurrentConfig.LoggingTimerInterval, LoggingTimerCallback);

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

            _temperatureManager?.Dispose();
            _sensorManager?.Dispose();
            _deviceManager?.Dispose();
            _effectManager?.Dispose();
            _speedControllerManager?.Dispose();
            _configManager?.Dispose();
            _cache?.Clear();

            _timerManager = null;
            _deviceManager = null;
            _temperatureManager = null;
            _sensorManager = null;
            _deviceManager = null;
            _effectManager = null;
            _speedControllerManager = null;
            _configManager = null;
            _cache = null;

            Dispose();
            IsDisposed = true;

            Logger.Info("Disposing done!");
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
                foreach (var profile in _configManager.CurrentConfig.ComputerStateProfiles.Where(p => p.StateType == state))
                {
                    foreach (var port in profile.Ports)
                    {
                        var controller = _deviceManager.GetController(port);
                        if (controller == null)
                            continue;

                        if(profile.Speed.HasValue)
                            controller.SetSpeed(port.Id, profile.Speed.Value);

                        var effectByte = controller.GetEffectByte(profile.EffectType);
                        if (effectByte.HasValue && profile.EffectColors != null)
                            controller.SetRgb(port.Id, effectByte.Value, profile.EffectColors);

                        if(state == ComputerStateType.Boot && (profile.Speed.HasValue || effectByte.HasValue))
                            controller.SaveProfile();
                    }
                }
            }
        }

        #region Timer Callbacks
        private bool TemperatureTimerCallback()
        {
            _temperatureManager.Update();
            _temperatureManager.Accept(_cache.AsWriteOnly());
            return true;
        }

        private bool DeviceSpeedTimerCallback()
        {
            var isCriticalTemperature = _configManager.CurrentConfig.CriticalTemperature.Any(pair =>
                _cache.GetTemperature(pair.Key) >= pair.Value);

            foreach (var profile in _configManager.CurrentConfig.Profiles)
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
                if (isCriticalTemperature)
                {
                    speedMap = profile.Ports.ToDictionary(p => p, _ => (byte)100);
                }
                else
                {
                    var speedControllers = _speedControllerManager.GetSpeedControllers(profile.Guid);
                    var speedController = speedControllers?.FirstOrDefault(c => c.IsEnabled(_cache));
                    if (speedController == null)
                        continue;

                    speedMap = speedController.GenerateSpeeds(profile.Ports, _cache.AsReadOnly());
                }

                if (speedMap == null)
                    continue;

                lock (_deviceManager)
                {
                    foreach (var (port, speed) in speedMap)
                    {
                        var controller = _deviceManager.GetController(port);
                        if (controller == null)
                            continue;

                        controller.SetSpeed(port.Id, speed);
                    }
                }
            }

            return true;
        }

        public bool DeviceRgbTimerCallback()
        {
            foreach (var profile in _configManager.CurrentConfig.Profiles)
            {
                var effects = _effectManager.GetEffects(profile.Guid);
                var effect = effects?.FirstOrDefault(e => e.IsEnabled(_cache));
                if (effect == null)
                    continue;

                var colorMap = effect.GenerateColors(profile.Ports, _cache.AsReadOnly());
                if (colorMap == null)
                    continue;

                foreach (var port in profile.Ports)
                {
                    var config = _cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    if (!colorMap.ContainsKey(port))
                        continue;

                    var colors = colorMap[port];

                    if (config.LedRotation > 0)
                        colors = colors.Skip(config.LedRotation).Concat(colors.Take(config.LedRotation)).ToList();
                    if (config.LedReverse)
                        colors.Reverse();
                    if (config.LedCount < colors.Count)
                        colors.RemoveRange(config.LedCount, colors.Count - config.LedCount);

                    colorMap[port] = colors;
                }

                lock (_deviceManager)
                {
                    foreach (var (port, colors) in colorMap)
                    {
                        if (colors == null)
                            continue;

                        var controller = _deviceManager.GetController(port);
                        var effectByte = controller?.GetEffectByte(effect.EffectType);
                        if (effectByte == null)
                            continue;

                        controller.SetRgb(port.Id, effectByte.Value, colors);
                    }
                }
            }

            return true;
        }

        public bool LoggingTimerCallback()
        {
            foreach (var profile in _configManager.CurrentConfig.Profiles)
            {
                foreach (var port in profile.Ports)
                {
                    var data = _cache.GetPortData(port);
                    if (data == null)
                        continue;

                    Logger.Debug("Port {0} data: {1}", port, data);
                }
            }

            lock (_temperatureManager)
            {
                foreach (var sensor in _sensorManager.TemperatureSensors)
                {
                    var value = _temperatureManager.GetSensorValue(sensor.Identifier);
                    if (float.IsNaN(value))
                        continue;
                    Logger.Debug("Sensor \"{0}\" value: {1}", sensor.Identifier, value);
                }
            }

            return true;
        }
        #endregion
    }
}
