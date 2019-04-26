using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using TTController.Common;

namespace TTController.Service.Manager
{
    public sealed class EffectManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<Guid, List<IEffectBase>> _effectsGuidMap;

        public EffectManager()
        {
            Logger.Info("Creating Effect Manager...");
            _effectsGuidMap = new Dictionary<Guid, List<IEffectBase>>();
        }

        public void Add(Guid guid, IEffectBase effect)
        {
            if(!_effectsGuidMap.ContainsKey(guid))
                _effectsGuidMap.Add(guid, new List<IEffectBase>());
            _effectsGuidMap[guid].Add(effect);

            Logger.Info("Adding effect: {0} [{1}]", effect.GetType().Name, guid);
        }

        public IReadOnlyList<IEffectBase> GetEffects(Guid guid)
        {
            if (!_effectsGuidMap.ContainsKey(guid))
                return null;

            return _effectsGuidMap[guid].AsReadOnly();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            Logger.Info("Disposing EffectManager...");

            var count = _effectsGuidMap.Values.Sum(l => l.Count);
            foreach (var effects in _effectsGuidMap.Values)
                foreach (var effect in effects)
                    effect.Dispose();

            Logger.Info("Disposed effects: {0}", count);

            _effectsGuidMap.Clear();
        }
    }
}
