using System;
using System.Collections.Generic;
using NLog;
using TTController.Common;

namespace TTController.Service.Manager
{
    public class SpeedControllerManager : IDisposable
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
            foreach (var controllers in _speedControllerGuidMap.Values)
                foreach (var controller in controllers)
                    controller.Dispose();
        }
    }
}
