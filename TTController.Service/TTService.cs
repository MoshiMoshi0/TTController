using System;
using System.Linq;
using System.ServiceProcess;
using TTController.Service.Config;
using TTController.Service.Hardware.Temperature;
using TTController.Service.Manager;
using TTController.Service.Speed.Controller;
using TTController.Service.Utils;

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
            _cache = new DataCache();
            _configManager = new ConfigManager("config.json");
            _configManager.LoadOrCreateConfig();
            _configManager.Visit(_cache);

            var alpha = Math.Exp(- _configManager.CurrentConfig.TemperatureTimerInterval / (double)_configManager.CurrentConfig.DeviceSpeedTimerInterval);
            var providerFactory = new MovingAverageTemperatureProviderFactory(alpha);
            _temperatureManager = new TemperatureManager(providerFactory);

            _effectManager = new EffectManager();
            _speedControllerManager = new SpeedControllerManager();
            _deviceManager = new DeviceManager();

            foreach (var profile in _configManager.CurrentConfig.Profiles)
            {
                foreach (var effect in profile.Effects)
                    _effectManager.CreateEffect(profile.Guid, effect.Type, effect.Config);

                foreach (var speedController in profile.SpeedControllers)
                    _speedControllerManager.CreateSpeedController(profile.Guid, speedController.Type, speedController.Config);

                foreach (var pwmSpeedController in _speedControllerManager.GetSpeedControllers(profile.Guid).OfType<PwmSpeedController>())
                    foreach (var sensor in pwmSpeedController.Config.Sensors)
                        _temperatureManager.EnableSensor(sensor);
            }

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_configManager.CurrentConfig.TemperatureTimerInterval, () =>
            {
                _temperatureManager.Update();
                _temperatureManager.Visit(_cache);
                return true;
            });
            _timerManager.RegisterTimer(_configManager.CurrentConfig.DeviceSpeedTimerInterval, () =>
            {
                lock (_deviceManager)
                {
                    foreach (var profile in _configManager.CurrentConfig.Profiles)
                    {
                        var speedControllers = _speedControllerManager.GetSpeedControllers(profile.Guid);
                        var speedController = speedControllers.FirstOrDefault(c => c.Enabled);
                        if (speedController == null)
                            continue;

                        foreach (var port in profile.Ports)
                        {
                            var controller = _deviceManager.GetController(port);
                            var data = controller?.GetPortData(port.Id);
                            _cache.StorePortData(port, data);
                        }

                        var speedMap = speedController.GenerateSpeeds(profile.Ports, _cache.GetProxy());
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
                lock (_deviceManager)
                {
                    foreach (var profile in _configManager.CurrentConfig.Profiles)
                    {
                        var effects = _effectManager.GetEffects(profile.Guid);
                        var effect = effects.FirstOrDefault(e => e.Enabled);
                        if (effect == null)
                            continue;

                        var colorMap = effect.GenerateColors(profile.Ports, _cache.GetProxy());
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
            _effectManager.Dispose();
            _speedControllerManager.Dispose();
            _configManager.Dispose();
            _cache.Clear();

            base.Dispose();
            IsDisposed = true;
        }
    }
}
