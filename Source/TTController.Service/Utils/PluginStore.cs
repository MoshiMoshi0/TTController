using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TTController.Common.Plugin;
using TTController.Service.Config.Data;

namespace TTController.Service.Utils
{
    public sealed class PluginStore : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, List<IPlugin>> _plugins;

        public PluginStore()
        {
            Logger.Info("Creating Plugin Store...");
            _plugins = new ConcurrentDictionary<string, List<IPlugin>>();
        }

        public void Add(ProfileData profile, IPlugin plugin)
        {
            if (!_plugins.ContainsKey(profile.Name))
                _plugins.TryAdd(profile.Name, new List<IPlugin>());

            _plugins[profile.Name].Add(plugin);
            Logger.Info("Adding plugin \"{0}\" [{1}]", plugin.GetType().Name, profile.Name);
        }

        public IEnumerable<IPlugin> Get(ProfileData profile) => Get<IPlugin>(profile);
        public IEnumerable<T> Get<T>(ProfileData profile) where T : IPlugin
            => _plugins.TryGetValue(profile.Name, out var value) ? value.OfType<T>() : Enumerable.Empty<T>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Plugin Store...");

            foreach (var (id, plugins) in _plugins)
            {
                foreach (var disposable in plugins.OfType<IDisposable>())
                {
                    disposable.Dispose();
                    Logger.Info("Disposing plugin \"{0}\" [{1}]", disposable.GetType().Name, id);
                }
            }

            _plugins.Clear();
        }
    }
}
