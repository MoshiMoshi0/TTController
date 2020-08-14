using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
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

            Sc($"failure {ServiceName} reset= 300 actions= restart/10000/restart/20000/restart/60000");
        }

        protected override void OnBeforeUninstall(IDictionary savedState)
        {
            using (var controller = new ServiceController(ServiceName))
            {
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("Shutting down the service...");
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }

            base.OnBeforeUninstall(savedState);
        }

        private void Sc(string args)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sc",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }
            };

            Console.WriteLine($"Executing: \"{process.StartInfo.FileName} {process.StartInfo.Arguments}\"");
            process.OutputDataReceived += (s, e) => { if (!e.Data?.StartsWith("\n") ?? false) Console.WriteLine(e.Data); };
            process.ErrorDataReceived += (s, e) => { if (!e.Data?.StartsWith("\n") ?? false) Console.WriteLine(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}
