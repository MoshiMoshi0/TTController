using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TTController.Service.Speed;

namespace TTController.Service.Manager
{
    public class SpeedControllerManager : IDisposable
    {
        private readonly TemperatureManager _temperatureManager;
        private readonly Dictionary<string, Type> _speedControllerTypeMap;
        private readonly Dictionary<Guid, List<ISpeedControllerBase>> _speedControllerGuidMap;

        public SpeedControllerManager(TemperatureManager temperatureManager)
        {
            _temperatureManager = temperatureManager;
            _speedControllerTypeMap = Assembly.GetAssembly(typeof(ISpeedControllerBase))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ISpeedControllerBase).IsAssignableFrom(t))
                .ToDictionary(t => t.Name, t => t, StringComparer.OrdinalIgnoreCase);

            _speedControllerGuidMap = new Dictionary<Guid, List<ISpeedControllerBase>>();
        }

        public void CreateSpeedController(Guid guid, string name, dynamic config)
        {
            if (!_speedControllerTypeMap.ContainsKey(name))
                return;

            var type = _speedControllerTypeMap[name];
            var speedController = (ISpeedControllerBase)Activator.CreateInstance(type, new object[] { _temperatureManager, config });

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
