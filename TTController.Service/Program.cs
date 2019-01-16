using System;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using TTController.Service.Manager;

namespace TTController.Service
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                if (args.Length > 0)
                {
                    if(args[0] == "--console")
                        new TTService().Initialize();
                    else if (args[0] == "--info")
                        ShowInfo();

                    Console.ReadKey();
                }
                else
                {
                    ManageService();
                }
            }
            else
            {
                ServiceBase.Run(new TTService());
            }
        }

        private static void ManageService()
        {
            var installed = ServiceController.GetServices().Any(s => s.ServiceName.Equals(TTInstaller.ServiceName));
            if (installed)
                UninstallService();
            else
                InstallService();
        }

        private static void ShowInfo()
        {
            var deviceManager = new DeviceManager();
            foreach (var controller in deviceManager.Controllers)
            {
                Console.WriteLine($"Name: {controller.Name}" +
                                  $"\nVendorId: {controller.VendorId}" +
                                  $"\nProductId: {controller.ProductId}" + 
                                  $"\nPorts:");
                foreach (var port in controller.Ports)
                {
                    var data = controller.GetPortData(port.Id);
                    Console.WriteLine($"\tId: {port.Id}" +
                                      $"\n\tStats:" +
                                      $"\n\t\tSpeed: {data.Speed}%" +
                                      $"\n\t\tRPM: {data.Rpm} RPM" +
                                      $"\n\t\tUnknown: {data.Unknown}" +
                                      $"\n\tIdentifier: [{port.ControllerVendorId}, {port.ControllerProductId}, {port.Id}]" +
                                      $"\n");
                }
            }
        }

        private static void InstallService()
        {
            if (AskChoice("Service not found. Install?", 'y', 'n') == 'y')
            {
                if (AskChoice("Install as custom user?", 'y', 'n') == 'y')
                    TTInstaller.Account = ServiceAccount.User;

                ManagedInstallerClass.InstallHelper(new[] { "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location });
            }
        }

        private static void UninstallService()
        {
            if (AskChoice("Service already installed. Uninstall?", 'y', 'n') == 'y')
                ManagedInstallerClass.InstallHelper(new[] { "/u", "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location });
        }

        private static char AskChoice(string message, params char[] choices)
        {
            while (true)
            {
                Console.Write($"{message} [{string.Join(", ", choices)}]");
                var keyInfo = Console.ReadKey(true);

                if (choices.Contains(keyInfo.KeyChar))
                {
                    Console.WriteLine($": {keyInfo.KeyChar}");
                    return keyInfo.KeyChar;
                }
            }
        }
    }
}
