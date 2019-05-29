using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;
using System.ServiceProcess;
using TTController.Service.Utils;

namespace TTController.Service
{
    [RunInstaller(true)]
    public class TTInstaller : Installer
    {
        public static readonly string ServiceName = "TTController";
        public static readonly string DisplayName = "Thermaltake Controller";
        public static readonly string Description = "This service is used to control Thermaltake devices";

        private readonly ServiceAccount _serviceAccount;

        public TTInstaller()
        {
            if (!Enum.TryParse(AppSettingsHelper.ReadValue("service-install-as"), true, out _serviceAccount))
                _serviceAccount = ServiceAccount.LocalSystem;

            var process = new ServiceProcessInstaller
            {
                Account = _serviceAccount
            };
            var service = new ServiceInstaller
            {
                ServiceName = ServiceName,
                DisplayName = DisplayName,
                Description = Description,
                StartType = ServiceStartMode.Automatic,
            };

            Installers.Add(process);
            Installers.Add(service);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            if (_serviceAccount == ServiceAccount.LocalSystem)
            {
                using (var service = new ManagementObject($"WIN32_Service.Name='{ServiceName}'"))
                {
                    var paramList = new object[11];
                    paramList[5] = true;
                    service.InvokeMethod("Change", paramList);
                }
            }
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            using (var sc = new ServiceController(ServiceName))
            {
                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("Shutting down the service...");
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }

            base.OnBeforeUninstall(savedState);
        }
    }
}
