using System;
using System.Collections.Generic;
using TTController.Service.Speed;

namespace TTController.Service.Manager
{
    public class SpeedControllerManager : IDisposable
    {
        private readonly Dictionary<Guid, List<ISpeedControllerBase>> _speedControllerGuidMap;

        public SpeedControllerManager()
        {
            _speedControllerGuidMap = new Dictionary<Guid, List<ISpeedControllerBase>>();
        }

        public void CreateSpeedController(Guid guid, Type type, SpeedControllerConfigBase config)
        {
            var speedController = (ISpeedControllerBase)Activator.CreateInstance(type, new object[] { config });

            if (!_speedControllerGuidMap.ContainsKey(guid))
                _speedControllerGuidMap.Add(guid, new List<ISpeedControllerBase>());
            _speedControllerGuidMap[guid].Add(speedController);
        }

        public List<ISpeedControllerBase> GetSpeedControllers(Guid guid)
        {
            if (!_speedControllerGuidMap.ContainsKey(guid))
                return null;

            return _speedControllerGuidMap[guid];
        }

        public void Dispose()
        {
            foreach (var controllers in _speedControllerGuidMap.Values)
                foreach (var controller in controllers)
                    controller.Dispose();
        }
    }
}
