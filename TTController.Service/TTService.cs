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

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_configManager.CurrentConfig.TemperatureTimerInterval, () =>
            {
                lock (_temperatureManager)
                    _temperatureManager.Update();
                return true;
            });
            _timerManager.RegisterTimer(_configManager.CurrentConfig.DeviceSpeedTimerInterval, () =>
            {
                lock(_deviceManager) lock (_configManager) lock(_temperatureManager)
                {
                    foreach (var profile in _configManager.CurrentConfig.Profiles)
                    {
                        foreach (var port in profile.Ports)
                        {
                            var controller = _deviceManager.GetController(port);
                            if (controller == null)
                                continue;
                        }
                    }
                }

                return true;
            });
            _timerManager.RegisterTimer(_configManager.CurrentConfig.DeviceRgbTimerInterval, () =>
            {
                lock(_deviceManager) lock (_configManager)
                {
                    foreach (var profile in _configManager.CurrentConfig.Profiles)
                    {
                        foreach (var port in profile.Ports)
                        {
                            var controller = _deviceManager.GetController(port);
                            if (controller == null)
                                continue;
                        }
                    }
                }

                return true;
            });

            _timerManager.Start();
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
