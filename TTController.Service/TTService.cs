using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using OpenHardwareMonitor.Hardware;
using TTController.Common;
using TTController.Common.Config;
using TTController.Service.Hardware.Temperature;
using TTController.Service.Manager;

namespace TTController.Service
{
    class TTService : ServiceBase
    {
        private DeviceManager _deviceManager;
        private ConfigManager _configManager;
        private TemperatureManager _temperatureManager;
        private TimerManager _timerManager;
        private EffectManager _effectManager;
        private SpeedControllerManager _speedControllerManager;

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
            _deviceManager = new DeviceManager();
            _configManager = new ConfigManager("config.json");
            _configManager.LoadOrCreateConfig();

            var alpha = Math.Exp(- _configManager.CurrentConfig.TemperatureTimerInterval / (double)_configManager.CurrentConfig.DeviceSpeedTimerInterval);
            var providerFactory = new MovingAverageTemperatureProviderFactory(alpha);
            _temperatureManager = new TemperatureManager(providerFactory);
            _temperatureManager.EnableSensor(new Identifier("intelcpu", "0", "temperature", "8"));

            _effectManager = new EffectManager();
            _speedControllerManager = new SpeedControllerManager(_temperatureManager);

            foreach (var profile in _configManager.CurrentConfig.Profiles)
            {
                foreach (var kv in profile.Effects)
                    _effectManager.CreateEffect(profile.Guid, kv.Key, kv.Value);
                foreach (var kv in profile.SpeedControllers)
                    _speedControllerManager.CreateSpeedController(profile.Guid, kv.Key, kv.Value);
            }

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_configManager.CurrentConfig.TemperatureTimerInterval, () =>
            {
                lock (_temperatureManager)
                    _temperatureManager.Update();
                return true;
            });
            _timerManager.RegisterTimer(_configManager.CurrentConfig.DeviceSpeedTimerInterval, () =>
            {
                lock(_temperatureManager)
                {
                    foreach (var profile in _configManager.CurrentConfig.Profiles)
                    {
                        var speedControllers = _speedControllerManager.GetSpeedControllers(profile.Guid);
                        var speedController = speedControllers.FirstOrDefault(c => c.Enabled);
                        if (speedController == null)
                            continue;

                        var portDataMap = profile.Ports
                            .ToDictionary(p => p, p => {
                                var controller = _deviceManager.GetController(p);
                                return controller?.GetPortData(p.Id);
                            })
                            .Where(kv => kv.Value != null)
                            .ToDictionary(kv => kv.Key, kv => kv.Value);

                        var speedMap = speedController.GenerateSpeeds(portDataMap);
                        foreach (var pair in speedMap)
                        {
                            var controller = _deviceManager.GetController(pair.Key);
                            if (controller == null)
                                continue;

                            Console.WriteLine($"{pair.Key} {pair.Value}");
                            controller.SetSpeed(pair.Key.Id, pair.Value);
                        }
                    }
                }

                return true;
            });
            _timerManager.RegisterTimer(_configManager.CurrentConfig.DeviceRgbTimerInterval, () =>
            {
                {
                    foreach (var profile in _configManager.CurrentConfig.Profiles)
                    {
                        var effects = _effectManager.GetEffects(profile.Guid);
                        var effect = effects.FirstOrDefault(e => e.Enabled);
                        if(effect == null)
                            continue;

                        var portConfigs = profile.Ports.ToDictionary(p => p, p => _configManager.CurrentConfig.PortConfig.GetValueOrDefault(p, PortConfigData.Default));
                        var colorMap = effect.GenerateColors(portConfigs);
                            
                        foreach (var pair in colorMap)
                        {
                            var controller = _deviceManager.GetController(pair.Key);
                            if (controller == null)
                                continue;

                            controller.SetRgb(pair.Key.Id, effect.EffectByte, pair.Value);
                        }
                    }
                }

                return true;
            });

            _timerManager.Start();
            Console.ReadKey();
            return true;
        }

        protected override void OnStart(string[] args)
        {
            if (!Initialize())
            {
                ExitCode = 1;
                Stop();
                throw new Exception("Service failed to start!");
            }

            IsDisposed = false;
        }

        protected override void OnStop()
        {
            Dispose();
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            Dispose();
            base.OnShutdown();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            switch (powerStatus)
            {
                case PowerBroadcastStatus.QuerySuspendFailed:
                    OnStart(null);
                    break;

                case PowerBroadcastStatus.ResumeAutomatic:
                case PowerBroadcastStatus.ResumeCritical:
                case PowerBroadcastStatus.ResumeSuspend:
                    OnStart(null);
                    break;

                case PowerBroadcastStatus.QuerySuspend:
                case PowerBroadcastStatus.Suspend:
                    OnStop();
                    break;

                default:
                    break;
            }

            return base.OnPowerEvent(powerStatus);
        }

        public new void Dispose()
        {
            if (IsDisposed)
                return;
            
            _timerManager.Dispose();
            _temperatureManager.Dispose();
            _deviceManager.Dispose();

            base.Dispose();
            IsDisposed = true;
        }
    }
}
