using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using TTController.Common.Plugin;

namespace TTController.Service.Utils
{
    [Serializable]
    public class PluginLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Type[] PluginTypes = new [] {
                typeof(IPlugin)
        };

        internal static List<Assembly> LoadAll(string path) => Load(path, PluginTypes);
        internal static List<Assembly> Load(string path, params Type[] types)
        {
            return Search(path, types).Select(a =>
            {
                Logger.Info("Loading plugin assembly: {0} [{1}]", a.GetName().Name, a.GetName().Version);
                return Assembly.Load(a.GetName());
            }).ToList();
        }

        internal static IEnumerable<Assembly> SearchAll(string path) => Search(path, PluginTypes);
        internal static IEnumerable<Assembly> Search(string path, params Type[] types)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Could not find directory {path}");

            var appDomain = CreateTempAppDomain();
            try
            {
                var loaderType = typeof(PluginLoader);
                var loader = (PluginLoader)appDomain.CreateInstanceAndUnwrap(loaderType.Assembly.FullName, loaderType.FullName);
                return types.SelectMany(t => loader.Search(path, t));
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        private IEnumerable<Assembly> Search(string path, Type type)
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += (s, e) =>
                Assembly.ReflectionOnlyLoad(e.Name);

            var contractType = GetLoadFromContractType(type);
            foreach (var dllFile in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories)) {
                var assembly = Assembly.ReflectionOnlyLoadFrom(dllFile);
                var found = assembly.GetExportedTypes().Any(contractType.IsAssignableFrom);

                if (found)
                {
                    Logger.Trace("Found plugin assembly for type {0}: {1} [{2}]", type.Name, assembly.GetName().Name, assembly.GetName().Version);
                    yield return assembly;
                }
            }
        }

        private static Type GetLoadFromContractType(Type type)
        {
            var contractAssemblyLoadFrom = Assembly.ReflectionOnlyLoad(type.Assembly.FullName);
            var contractType = contractAssemblyLoadFrom.GetExportedTypes()
                .FirstOrDefault(x => x.FullName == type.FullName && x.Assembly.FullName == type.Assembly.FullName);

            if (contractType == null)
                throw new InvalidOperationException($"Could not find type {type.FullName} in the LoadFrom assemblies");

            return contractType;
        }

        private static AppDomain CreateTempAppDomain()
        {
            var appName = $"{AppDomain.CurrentDomain.FriendlyName}_TempPluginLoaderDomain";
            var domainSetup = new AppDomainSetup
            {
                ApplicationName = appName,
                ShadowCopyFiles = "false",
                ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                DynamicBase = AppDomain.CurrentDomain.SetupInformation.DynamicBase,
                LicenseFile = AppDomain.CurrentDomain.SetupInformation.LicenseFile,
                LoaderOptimization = AppDomain.CurrentDomain.SetupInformation.LoaderOptimization,
                PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath,
                PrivateBinPathProbe = AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe
            };

            return AppDomain.CreateDomain(
                appName, AppDomain.CurrentDomain.Evidence,
                domainSetup, new PermissionSet(PermissionState.Unrestricted)
            );
        }
    }
}
