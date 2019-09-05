using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using NLog;
using OpenHardwareMonitor.Hardware;
using TTController.Common.Plugin;
using TTController.Service.Config.Data;
using TTController.Service.Hardware;
using TTController.Service.Manager;
using TTController.Service.Utils;

namespace TTController.Service
{
    internal static class Program
    {
        private static ServiceController Service { get; set; }

        private static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                MainMenu();
            }
            else
            {
                ServiceBase.Run(new TTService());
            }
        }

        private static void MainMenu()
        {
            var menu = new MenuPage("Main Menu");
            menu.Add("Manage Service", ManageServiceMenu, () => true);
            menu.Add("Run in console", () => {
                Console.Clear();
                Console.ResetColor();
                var service = new TTService();
                service.Initialize();
                Console.ReadKey(true);
                service.Finalize(ComputerStateType.Shutdown);
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey(true);
                return false;
            }, () => Service == null || Service.Status != ServiceControllerStatus.Running);
            menu.Add("Debug", DebugMenu, () => Service != null && Service.Status != ServiceControllerStatus.Running);
            menu.Add("Exit", () => true, () => true, '0');

            while (true)
            {
                Console.Clear();
                var selected = menu.Show();
                if (selected.Callback())
                    return;
            }
        }

        private static bool ManageServiceMenu()
        {
            void StopService()
            {
                Console.WriteLine("Stopping the service...");
                Service?.Stop();
                Service?.WaitForStatus(ServiceControllerStatus.Stopped);
            }

            void StartService()
            {
                Console.WriteLine("Starting the service...");
                try
                {
                    Service?.Start();
                    Service?.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to start the service...");
                    Console.WriteLine(e);
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey(true);
                }
            }

            var menu = new MenuPage("Main Menu > Manage Service");
            menu.Add("Start", () =>
            {
                StartService();
                return false;
            }, () => Service != null && Service.Status != ServiceControllerStatus.Running);
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

                try
                {
                    ManagedInstallerClass.InstallHelper(new[]
                        {"/u", "/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location});
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to uninstall the service...");
                    Console.WriteLine(e);
                }

                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey(true);
                return false;
            }, () => Service != null);
            menu.Add("Install", () => {
                try
                {
                    ManagedInstallerClass.InstallHelper(new[]
                        {"/LogFile=", "/LogToConsole=true", Assembly.GetExecutingAssembly().Location});
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to install the service...");
                    Console.WriteLine(e);
                }

                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey(true);
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

        private static bool DebugMenu()
        {
            if (!LogManager.Configuration.LoggingRules.Any(r => r.IsLoggingEnabledForLevel(LogLevel.Trace)))
                LogManager.DisableLogging();

            void WriteHeader(string header)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(header);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("-------------------------------");
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            void WriteFooter()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("-------------------------------");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ResetColor();
            }

            void WriteProperty(int indent, string propertyName, object value = null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"» {new string('\t', indent)} ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{propertyName}");
                if (value != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{value}");
                }
                Console.WriteLine();
                Console.ResetColor();
            }

            void WaitForInput()
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            void ListControllers()
            {
                WriteHeader("Controllers");
                WriteProperty(0, "");

                PluginLoader.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"), typeof(IControllerDefinition));
                using (var deviceManager = new DeviceManager())
                {
                    foreach(var controller in deviceManager.Controllers)
                    {
                        WriteProperty(0, "Name: ", controller.Name);
                        WriteProperty(1, "VendorId: ", controller.VendorId);
                        WriteProperty(1, "ProductId: ", controller.ProductId);
                        WriteProperty(1, "Ports: ");

                        foreach (var port in controller.Ports)
                        {
                            var data = controller.GetPortData(port.Id);
                            WriteProperty(2, $"{port.Id}: ");
                            WriteProperty(3, "Data: ", data);
                            WriteProperty(3, "Identifier: ", port);
                        }

                        WriteProperty(1, "Available effect types: ", string.Join(", ", controller.EffectTypes));
                    }
                }

                WriteProperty(0, "");
                WriteFooter();
            }

            void ListSensors(params SensorType[] types)
            {
                WriteHeader("Sensors");
                WriteProperty(0, "");

                string FormatValue(SensorType type, float value)
                {
                    switch (type)
                    {
                        case SensorType.Voltage:     return $"{value:F2} V";
                        case SensorType.Clock:       return $"{value:F0} MHz";
                        case SensorType.Load:        return $"{value:F1} %";
                        case SensorType.Temperature: return $"{value:F1} °C";
                        case SensorType.Fan:         return $"{value:F0} RPM";
                        case SensorType.Flow:        return $"{value:F0} L/h";
                        case SensorType.Control:     return $"{value:F1} %";
                        case SensorType.Level:       return $"{value:F1} %";
                        case SensorType.Power:       return $"{value:F0} W";
                        case SensorType.Data:        return $"{value:F0} GB";
                        case SensorType.SmallData:   return $"{value:F1} MB";
                        case SensorType.Factor:      return $"{value:F3}";
                        case SensorType.Frequency:   return $"{value:F1} Hz";
                        case SensorType.Throughput:  return $"{value:F1} B/s";
                    }

                    return $"{value}";
                }

                using (var _openHardwareMonitorFacade = new OpenHardwareMonitorFacade())
                {
                    var availableSensors = _openHardwareMonitorFacade.Sensors.Where(s => types.Length > 0 ? types.Contains(s.SensorType) : true);
                    foreach (var (hardware, sensors) in availableSensors.GroupBy(s => s.Hardware))
                    {
                        WriteProperty(0, $"{hardware.Name}:");
                        hardware.Update();

                        foreach (var (type, group) in sensors.GroupBy(s => s.SensorType))
                        {
                            WriteProperty(1, $"{type}:");
                            foreach (var sensor in group)
                                WriteProperty(2, $"{ sensor.Name} ({ sensor.Identifier}): ", FormatValue(type, sensor.Value ?? float.NaN));
                        }

                        WriteProperty(0, "");
                    }
                }

                WriteFooter();
            }

            void ListPlugins()
            {
                WriteHeader("Plugins");
                WriteProperty(0, "");

                var pluginAssemblies = PluginLoader.SearchAll(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins"));
                WriteProperty(0, "Detected plugins:");
                foreach (var assembly in pluginAssemblies)
                    WriteProperty(1, Path.GetFileName(assembly.Location));

                WriteProperty(0, "");
                WriteFooter();
            }

            var enabled = Service != null && Service.Status != ServiceControllerStatus.Running;
            var menu = new MenuPage("Main Menu > Debug");
            menu.Add("Report", () => {
                Console.Clear();
                ListControllers();
                ListSensors(SensorType.Temperature);
                WaitForInput();
                return false;
            }, () => enabled);
            menu.Add("Controllers", () => {
                Console.Clear();
                ListControllers();
                WaitForInput();
                return false;
            }, () => enabled);
            menu.Add("Sensors", () => {
                Console.Clear();
                ListSensors();
                WaitForInput();
                return false;
            }, () => enabled);
            menu.Add("Plugins", () => {
                Console.Clear();
                ListPlugins();
                WaitForInput();
                return false;
            }, () => enabled);
            menu.Add("Back", () => {
                LogManager.EnableLogging();
                return true;
            }, () => true, '0');

            while (true)
            {
                Console.Clear();
                var selected = menu.Show();
                if (selected.Callback())
                    return false;
            }
        }

        #region Menu
        private class MenuOption
        {
            public MenuOption(string description, Func<bool> callback, Func<bool> enabled, char? keyOverride = null)
            {
                Description = description;
                Callback = callback;
                Enabled = enabled;
                KeyOverride = keyOverride;
            }

            public string Description { get; }
            public Func<bool> Callback { get; }
            public Func<bool> Enabled { get; }
            public char? KeyOverride { get; }
        }

        private class MenuPage
        {
            private readonly List<MenuOption> _options;
            private readonly string _header;

            public void Add(string description, Func<bool> callback, Func<bool> enabled, char? keyOverride = null) =>
                _options.Add(new MenuOption(description, callback, enabled, keyOverride));

            public MenuPage(string header = null)
            {
                _header = header;
                _options = new List<MenuOption>();
            }

            public MenuOption Show()
            {
                Service = ServiceController.GetServices()
                    .FirstOrDefault(s => s.ServiceName.Equals(TTInstaller.ServiceName));

                if (_header != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(_header);
                    Console.WriteLine("================================");
                }

                var index = 0;
                var optionMap = new Dictionary<char, MenuOption>();
                foreach (var option in _options)
                {
                    var key = option.KeyOverride ?? (char) (index++ + (index > 9 ? 'a' : '1'));
                    optionMap.Add(key, option);

                    Console.ForegroundColor = option.Enabled() ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                    Console.WriteLine("[{0}] {1}", key, option.Description);
                }

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{string.Join(", ", optionMap.Where(kv => kv.Value.Enabled()).Select(kv => kv.Key))}]: ");

                while (true)
                {
                    var c = Console.ReadKey(true).KeyChar;
                    if (optionMap.ContainsKey(c) && optionMap[c].Enabled())
                    {
                        Console.ResetColor();
                        Console.WriteLine(c);
                        return optionMap[c];
                    }
                }
            }
        }
        #endregion
    }
}
