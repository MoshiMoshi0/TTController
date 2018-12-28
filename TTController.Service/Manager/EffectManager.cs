using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TTController.Service.Rgb;

namespace TTController.Service.Manager
{
    public class EffectManager : IDisposable
    {
        private readonly Dictionary<string, Type> _effectTypeMap;
        private readonly Dictionary<Guid, List<IEffectBase>> _effectsGuidMap;

        public EffectManager()
        {
            _effectTypeMap = Assembly.GetAssembly(typeof(IEffectBase))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IEffectBase).IsAssignableFrom(t))
                .ToDictionary(t => t.Name, t => t, StringComparer.OrdinalIgnoreCase);

            _effectsGuidMap = new Dictionary<Guid, List<IEffectBase>>();
        }

        public void CreateEffect(Guid guid, string name, dynamic config)
        {
            if (!_effectTypeMap.ContainsKey(name))
                return;

            var type = _effectTypeMap[name];
            var effect = (IEffectBase) Activator.CreateInstance(type, new object[]{config});

            if(!_effectsGuidMap.ContainsKey(guid))
                _effectsGuidMap.Add(guid, new List<IEffectBase>());
            _effectsGuidMap[guid].Add(effect);
        }

        public List<IEffectBase> GetEffects(Guid guid)
        {
            if (!_effectsGuidMap.ContainsKey(guid))
                return null;

            return _effectsGuidMap[guid];
        }

        public void Dispose()
        {
            foreach (var effects in _effectsGuidMap.Values)
                foreach (var effect in effects)
                    effect.Dispose();
        }
    }
}
