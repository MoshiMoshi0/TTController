using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using TTController.Common.Plugin;

namespace TTController.Service.Manager
{
    public sealed class SpeedControllerManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Guid, List<ISpeedControllerBase>> _speedControllerGuidMap;

        public SpeedControllerManager()
        {
            Logger.Info("Creating Speed Controller Manager...");
            _speedControllerGuidMap = new Dictionary<Guid, List<ISpeedControllerBase>>();
        }

        public void Add(Guid guid, ISpeedControllerBase speedController)
        {
            if (!_speedControllerGuidMap.ContainsKey(guid))
                _speedControllerGuidMap.Add(guid, new List<ISpeedControllerBase>());
            _speedControllerGuidMap[guid].Add(speedController);

            Logger.Info("Adding speed controller: {0} [{1}]", speedController.GetType().Name, guid);
        }

        public IReadOnlyList<ISpeedControllerBase> GetSpeedControllers(Guid guid)
        {
            if (!_speedControllerGuidMap.ContainsKey(guid))
                return null;

            return _speedControllerGuidMap[guid].AsReadOnly();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing SpeedControllerManager...");

            var count = _speedControllerGuidMap.Values.Sum(l => l.Count);
            foreach (var controllers in _speedControllerGuidMap.Values)
                foreach (var controller in controllers)
                    controller.Dispose();

            Logger.Info("Disposed speed controllers: {0}", count);

            _speedControllerGuidMap.Clear();
        }
    }
}
