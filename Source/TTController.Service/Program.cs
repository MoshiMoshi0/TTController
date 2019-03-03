using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using OpenHardwareMonitor.Hardware;
using TTController.Service.Manager;
using TTController.Service.Utils;

namespace TTController.Service
{
    static class Program
    {
        private static ServiceController Service => ServiceController.GetServices()
            .FirstOrDefault(s => s.ServiceName.Equals(TTInstaller.ServiceName));

        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                var menu = new MenuPage("Main Menu");
                menu.Add("Manage Service", ManageService, () => true);
                menu.Add("Run in console", () => {
                    new TTService().Initialize();
                    Console.ReadKey();
                    return false;
                }, () => Service?.Status != ServiceControllerStatus.Running);
                menu.Add("Show hardware info", () => {
                    ShowInfo();
                    return false;
                }, () => Service?.Status != ServiceControllerStatus.Running);
                menu.Add("Exit", () => true, () => true, '0');

                while (true)
                {
                    Console.Clear();
                    var selected = menu.Show();
                    if (selected.Callback())
                        return;
                }
            }
            else
            {
                ServiceBase.Run(new TTService());
            }
        }

        private static bool ManageService()
        {
            void StopService()
            {
                Service?.Stop();
                Service?.WaitForStatus(ServiceControllerStatus.Stopped);
            }

            void StartService()
            {
                Service?.Start();
                Service?.WaitForStatus(ServiceControllerStatus.Running);
            }

            var menu = new MenuPage("Main Menu > Manage Service");
            menu.Add("Start", () =>
            {
                StartService();
                return false;
            }, () => Service?.Status != ServiceControllerStatus.Running);
            menu.Add("Stop", () =>
            {
                StopService();
                return false;
            }, () => Service?.Status == ServiceControllerStatus.Running);
            menu.Add("Restart", () => {
                StopService();
                StartService();
                return false;
            }, () => Service?.Status == ServiceControllerStatus.Running);
            menu.Add("Uninstall", () => {
                if(Service?.Status != ServiceControllerStatus.Stopped)
                    StopService();
                ManagedInstallerClass.InstallHelper(new[]
                    {"/u", "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location});
                return false;
            }, () => Service != null);
            menu.Add("Install", () => {
                ManagedInstallerClass.InstallHelper(new[]
                    {"/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location});
                return false;
            }, () => Service == null);
            menu.Add("Back", () => true, () => true, '0');
            
            while (true)
            {
                Console.Clear();
                var selected = menu.Show();
                if (selected.Callback())
                    return false;
            }
        }

        private static void ShowInfo()
        {
            Console.Clear();
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
                                          $"\n\tData: {data}" +
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
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        #region Menu
        private class MenuOption
        {
            public MenuOption(string description, Func<bool> callback, Func<bool> enabled, char key = Char.MaxValue)
            {
                Description = description;
                Callback = callback;
                Enabled = enabled;
                Key = key;
            }

            public string Description { get; }
            public Func<bool> Callback { get; }
            public Func<bool> Enabled { get; }
            public char Key { get; }
        }

        private class MenuPage
        {
            private readonly IList<MenuOption> _options;
            private readonly string _header;

            public void Add(string description, Func<bool> callback, Func<bool> enabled, char key = Char.MaxValue) =>
                _options.Add(new MenuOption(description, callback, enabled, key));

            public MenuPage(string header = null)
            {
                _header = header;
                _options = new List<MenuOption>();
            }

            public MenuOption Show()
            {
                if (_header != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(_header);
                    Console.WriteLine("================================");
                }

                var index = 1;
                var format = "[{0}] {1}";
                var optionMap = _options
                    .Where(o => o.Enabled())
                    .ToDictionary(o => o.Key == char.MaxValue ? (char)(index++ + '0') : o.Key, o => o);

                Console.ForegroundColor = ConsoleColor.Gray;
                foreach (var (key, option) in optionMap)
                    Console.WriteLine(format, key, option.Description);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Disabled options:");
                foreach (var option in _options.Where(o => !o.Enabled()))
                    Console.WriteLine(format, 'x', option.Description);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{string.Join(", ", optionMap.Keys)}]: ");
                
                while (true)
                {
                    var keyInfo = Console.ReadKey(true);

                    if (optionMap.Keys.Contains(keyInfo.KeyChar))
                    {
                        Console.ResetColor();
                        return optionMap[keyInfo.KeyChar];
                    }
                }
            }
        }
        #endregion
    }
}
