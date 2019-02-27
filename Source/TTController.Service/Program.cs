using System;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using OpenHardwareMonitor.Hardware;
using TTController.Service.Manager;

namespace TTController.Service
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                switch (AskChoice(  "[1]\tManage Service" + 
                                  "\n[2]\tRun in console" +
                                  "\n[3]\tShow hardware info" + 
                                  "\n", '1', '2', '3'))
                {
                    case '1':
                        ManageService();
                        break;
                    case '2':
                        new TTService().Initialize();
                        Console.ReadKey();
                        break;
                    case '3':
                        ShowInfo();
                        Console.ReadKey();
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new TTService());
            }
        }

        private static void ManageService()
        {
            Console.WriteLine("\n-----------------\n");

            var service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.Equals(TTInstaller.ServiceName));
            void Start()
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
            void Stop()
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
            }
            ServiceController Install()
            {
                ManagedInstallerClass.InstallHelper(new[]
                    {"/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location});
                return new ServiceController(TTInstaller.ServiceName);
            }
            void Uninstall()
            {
                Stop();
                ManagedInstallerClass.InstallHelper(new[]
                    {"/u", "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location});
            }
            void Restart() { Stop(); Start(); }

            if (service != null)
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    switch (AskChoice("[1]\tStop\n[2]\tRestart\n[3]\tUninstall\n", '1', '2', '3'))
                    {
                        case '1': Stop(); break;
                        case '2': Restart(); break;
                        case '3': Uninstall(); break;
                    }
                }
                else
                {
                    switch (AskChoice("[1]\tStart\n[2]\tUninstall\n", '1', '2'))
                    {
                        case '1': Start(); break;
                        case '2': Uninstall(); break;
                    }
                }
            }
            else
            {
                if (AskChoice("Service not found. Install? ", 'y', 'n') == 'y')
                {
                    service = Install();
                    Start();
                }
            }
        }

        private static void ShowInfo()
        {
            Console.WriteLine("Controllers");
            Console.WriteLine("-------------------------------");
            using (var deviceManager = new DeviceManager())
            {
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
                                          $"\n\tIdentifier: {port}" +
                                          $"\n");
                    }
                }
            }

            Console.WriteLine("Sensors");
            Console.WriteLine("-------------------------------");
            using (var temperatureManager = new TemperatureManager(null))
            {
                foreach (var hardware in temperatureManager.Sensors.Select(s => s.Hardware).Distinct())
                {
                    hardware.Update();

                    Console.WriteLine($"{hardware.Name}:");
                    foreach (var sensor in hardware.Sensors.Where(s => s.SensorType == SensorType.Temperature))
                        Console.WriteLine($"\t{sensor.Identifier}:" +
                                          $"\n\t\tName: {sensor.Name}" + 
                                          $"\n\t\tValue: {sensor.Value ?? float.NaN}" +
                                          $"\t");
                }
            }
        }

        private static char AskChoice(string message, params char[] choices)
        {
            while (true)
            {
                Console.Write($"{message}[{string.Join(", ", choices)}]: ");
                var keyInfo = Console.ReadKey(true);

                if (choices.Contains(keyInfo.KeyChar))
                {
                    Console.WriteLine($"{keyInfo.KeyChar}");
                    return keyInfo.KeyChar;
                }

                Console.WriteLine();
            }
        }
    }
}
