using System;
using System.ServiceProcess;
using System.Threading;
using OpenHardwareMonitor.Hardware;
using TTController.Service.Hardware.Temperature;
using TTController.Service.Managers;

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

            var alpha = Math.Exp(_configManager.Config.TemperatureTimerInterval / (double)_configManager.Config.DeviceSpeedTimerInterval);
            var providerFactory = new MovingAverageTemperatureProviderFactory(alpha);
            _temperatureManager = new TemperatureManager(providerFactory);
            _temperatureManager.EnableSensor(new Identifier("intelcpu", "0", "temperature", "8"));

            _timerManager = new TimerManager();
            _timerManager.RegisterTimer(_configManager.Config.TemperatureTimerInterval, () =>
            {
                lock (_temperatureManager)
                    _temperatureManager.Update();
                return true;
            });
            _timerManager.Start();

            Thread.Sleep(10000);
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
            
            base.Dispose();
            IsDisposed = true;
        }
    }
}
