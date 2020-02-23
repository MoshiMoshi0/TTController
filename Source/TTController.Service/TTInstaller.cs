using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;
using System.ServiceProcess;

namespace TTController.Service
{
    [RunInstaller(true)]
    public class TTInstaller : Installer
    {
        internal static readonly string ServiceName = "TTController";
        internal static readonly string DisplayName = "Thermaltake Controller";
        internal static readonly string Description = "This service is used to control Thermaltake devices";
        internal static readonly ServiceAccount ServiceAccount = ServiceAccount.LocalSystem;

        public TTInstaller()
        {
            var process = new ServiceProcessInstaller
            {
                Account = ServiceAccount
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

            using (var service = new ManagementObject($"WIN32_Service.Name='{ServiceName}'"))
            {
                var paramList = new object[11];
                paramList[5] = true;
                service.InvokeMethod("Change", paramList);
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
