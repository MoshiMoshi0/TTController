using System;
using System.Collections.Generic;
using TTController.Service.Rgb;

namespace TTController.Service.Utils
{
    public static class Extensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static bool HasSpeed(this EffectType effectType)
        {
            return effectType != EffectType.ByLed && effectType != EffectType.Full;
        }
    }
}
