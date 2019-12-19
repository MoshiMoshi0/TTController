using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TTController.Common.Plugin;

namespace TTController.Service.Utils
{
    public sealed class PluginStore : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<Guid, List<IPlugin>> _plugins;

        public PluginStore()
        {
            Logger.Info("Creating Plugin Store...");
            _plugins = new ConcurrentDictionary<Guid, List<IPlugin>>();
        }

        public void Add(Guid guid, IPlugin plugin)
        {
            if (!_plugins.ContainsKey(guid))
                _plugins.TryAdd(guid, new List<IPlugin>());

            _plugins[guid].Add(plugin);
            Logger.Info("Adding plugin \"{0}\" [{1}]", plugin.GetType().Name, guid);
        }

        public IEnumerable<IPlugin> Get(Guid guid) => Get<IPlugin>(guid);
        public IEnumerable<T> Get<T>(Guid guid) where T : IPlugin
            => _plugins.TryGetValue(guid, out var value) ? value.OfType<T>() : Enumerable.Empty<T>();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing Plugin Store...");

            foreach (var (guid, plugins) in _plugins)
            {
                foreach (var disposable in plugins.OfType<IDisposable>())
                {
                    disposable.Dispose();
                    Logger.Info("Disposing plugin \"{0}\" [{1}]", disposable.GetType().Name, guid);
                }
            }

            _plugins.Clear();
        }
    }
}
