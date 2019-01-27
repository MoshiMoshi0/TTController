using System;
using System.Collections.Generic;
using TTController.Common;

namespace TTController.Service.Manager
{
    public class SpeedControllerManager : IDisposable
    {
        private readonly Dictionary<Guid, List<ISpeedControllerBase>> _speedControllerGuidMap;

        public SpeedControllerManager()
        {
            _speedControllerGuidMap = new Dictionary<Guid, List<ISpeedControllerBase>>();
        }

        public void Add(Guid guid, ISpeedControllerBase speedController)
        {
            if (!_speedControllerGuidMap.ContainsKey(guid))
                _speedControllerGuidMap.Add(guid, new List<ISpeedControllerBase>());
            _speedControllerGuidMap[guid].Add(speedController);
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
