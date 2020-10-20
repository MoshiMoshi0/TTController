using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TTController.Common;
using TTController.Common.Plugin;
using TTController.Service.Config;

namespace TTController.Service.Utils
{
    public sealed class PluginStore : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, List<IPlugin>> _profilePlugins;
        private readonly ConcurrentBag<IPlugin> _plugins;

        public PluginStore() : this(Enumerable.Empty<IPlugin>()) { }
        public PluginStore(IEnumerable<IPlugin> plugins)
        {
            Logger.Info("Creating Plugin Store...");
            _plugins = new ConcurrentBag<IPlugin>(plugins);

            foreach (var plugin in _plugins)
                Logger.Debug("Loaded plugin \"{0}\"", plugin.GetType().Name);

            Logger.Info("Loaded {0} plugins", _plugins.Count);

            _profilePlugins = new ConcurrentDictionary<string, List<IPlugin>>();
        }

        public void Assign(IPlugin plugin, ProfileConfig profile)
        {
            if (!_plugins.Contains(plugin))
                return;
            if (!_profilePlugins.ContainsKey(profile.Name))
                _profilePlugins.TryAdd(profile.Name, new List<IPlugin>());

            _profilePlugins[profile.Name].Add(plugin);
            Logger.Debug("Assigned plugin \"{0}\" to \"{1}\" profile", plugin.GetType().Name, profile.Name);
        }

        public void Add(IPlugin plugin) => _plugins.Add(plugin);
        public IEnumerable<IPlugin> Get(ProfileConfig profile) => Get<IPlugin>(profile);
        public IEnumerable<T> Get<T>(ProfileConfig profile) where T : IPlugin
            => _profilePlugins.TryGetValue(profile.Name, out var value) ? value.OfType<T>() : Enumerable.Empty<T>();
        public void Get<T>() where T : IPlugin => _profilePlugins.SelectMany(x => x.Value).OfType<T>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Plugin Store...");
            foreach (var disposable in _plugins.OfType<IDisposable>())
            {
                Logger.Info("Disposing plugin \"{0}\"", disposable.GetType().Name);
                disposable.Dispose();
            }

            _profilePlugins.Clear();
            while (_plugins.TryTake(out var _)) ;
        }
    }
}
