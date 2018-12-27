using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TTController.Service.Rgb;

namespace TTController.Service.Manager
{
    public class EffectManager
    {
        private readonly Dictionary<string, Type> _effectTypeMap;
        private readonly Dictionary<Guid, EffectBase> _effects;

        public EffectManager()
        {
            _effectTypeMap = Assembly.GetAssembly(typeof(EffectBase))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(EffectBase)))
                .ToDictionary(t => t.Name, t => t);

            _effects = new Dictionary<Guid, EffectBase>();
        }

        public void CreateEffect(Guid guid, string name, dynamic config)
        {
            if (!_effectTypeMap.ContainsKey(name))
                return;

            var type = _effectTypeMap[name];
            var effect = (EffectBase) Activator.CreateInstance(type, new object[]{config});
            _effects.Add(guid, effect);
        }

        public EffectBase GetEffect(Guid guid)
        {
            if (!_effects.ContainsKey(guid))
                return null;

            return _effects[guid];
        }
    }
}
