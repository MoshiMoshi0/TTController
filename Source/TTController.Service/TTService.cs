using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using NLog;
using TTController.Common;
using TTController.Service.Config.Data;
using TTController.Service.Hardware.Sensor;
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
            PluginLoader.LoadAll($@"{AppDomain.CurrentDomain.BaseDirectory}\Plugins");

            const string key = "config-file";
            if (string.IsNullOrEmpty(AppSettingsHelper.ReadValue(key)))
                AppSettingsHelper.WriteValue(key, "config.json");

            _configManager = new ConfigManager(AppSettingsHelper.ReadValue(key));
            if (!_configManager.LoadOrCreateConfig())
                return false;

            _cache = new DataCache();

            var alpha = Math.Exp(-_configManager.CurrentConfig.SensorTimerInterval / (double)_configManager.CurrentConfig.DeviceSpeedTimerInterval);
            var providerFactory = new MovingAverageSensorValueProviderFactory(alpha);
            var sensorConfigs = _configManager.CurrentConfig.SensorConfigs
                .SelectMany(x => x.Sensors.Select(s => (Sensor: s, Config: x.Config)))
                .ToDictionary(x => x.Sensor, x => x.Config);

            _sensorManager = new SensorManager(providerFactory, sensorConfigs);
            _effectManager = new EffectManager();
            _speedControllerManager = new SpeedControllerManager();
            _deviceManager = new DeviceManager();

            _sensorManager.EnableSensors(sensorConfigs.Keys);
            foreach (var profile in _configManager.CurrentConfig.Profiles)
            {
                foreach (var effect in profile.Effects)
                    _effectManager.Add(profile.Guid, effect);

                foreach (var speedController in profile.SpeedControllers)
                    _speedControllerManager.Add(profile.Guid, speedController);

                _sensorManager.EnableSensors(_speedControllerManager.GetSpeedControllers(profile.Guid)?.SelectMany(c => c.UsedSensors));
                _sensorManager.EnableSensors(_effectManager.GetEffects(profile.Guid)?.SelectMany(e => e.UsedSensors));
            }

            _sensorManager.Accept(_cache.AsWriteOnly());
            _deviceManager.Accept(_cache.AsWriteOnly());
            _configManager.Accept(_cache.AsWriteOnly());

            ApplyComputerStateProfile(ComputerStateType.Boot);

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_configManager.CurrentConfig.SensorTimerInterval, SensorTimerCallback);
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

            _sensorManager?.Dispose();
            _deviceManager?.Dispose();
            _effectManager?.Dispose();
            _speedControllerManager?.Dispose();
            _configManager?.Dispose();
            _cache?.Clear();

            _timerManager = null;
            _deviceManager = null;
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
                if (criticalState)
                {
                    speedMap = profile.Ports.ToDictionary(p => p, _ => (byte)100);
                }
                else
                {
                    var speedControllers = _speedControllerManager.GetSpeedControllers(profile.Guid);
                    var speedController = speedControllers?.FirstOrDefault(c => c.IsEnabled(_cache.AsReadOnly()));
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
            void ApplyConfig(IDictionary<PortIdentifier, List<LedColor>> colorMap)
            {
                foreach (var port in colorMap.Keys.ToList())
                {
                    var config = _cache.GetPortConfig(port);
                    if (config == null)
                        continue;

                    var colors = colorMap[port];

                    if (config.LedRotation > 0)
                        colors = colors.Skip(config.LedRotation).Concat(colors.Take(config.LedRotation)).ToList();
                    if (config.LedReverse)
                        colors.Reverse();

                    switch (config.LedCountHandling)
                    {
                        case LedCountHandling.Lerp:
                            {
                                if (config.LedCount == colors.Count)
                                    break;

                                var newColors = new List<LedColor>();
                                var gradient = new LedColorGradient(colors, config.LedCount - 1);

                                for (var i = 0; i < config.LedCount; i++)
                                    newColors.Add(gradient.GetColor(i));

                                colors = newColors;
                                break;
                            }
                        case LedCountHandling.Trim:
                            if (config.LedCount < colors.Count)
                                colors.RemoveRange(config.LedCount, colors.Count - config.LedCount);
                            break;
                        case LedCountHandling.Copy:
                            while (config.LedCount > colors.Count)
                                colors.AddRange(colors.Take(config.LedCount - colors.Count));
                            break;
                        case LedCountHandling.DoNothing:
                        default:
                            break;
                    }

                    colorMap[port] = colors;
                }
            }

            foreach (var profile in _configManager.CurrentConfig.Profiles)
            {
                var effects = _effectManager.GetEffects(profile.Guid);
                var effect = effects?.FirstOrDefault(e => e.IsEnabled(_cache.AsReadOnly()));
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

                        var controller = _deviceManager.GetController(port);
                        var effectByte = controller?.GetEffectByte(effectType);
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
