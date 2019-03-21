using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<Type> FindInAssemblies(this Type type)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract);

            if (type.IsInterface)
                return types.Where(t => type.IsAssignableFrom(t));
            else
                return types.Where(t => t.IsSubclassOf(type));
        }

        public static IEnumerable<TResult> TrySelect<TSource, TResult>(
            this IEnumerable<TSource> enumerable, 
            Func<TSource, TResult> selector, 
            Action<Exception> exceptionAction)
        {
            foreach (var item in enumerable)
            {
                TResult result = default(TResult);
                bool success = false;
                try
                {
                    result = selector(item);
                    success = true;
                }
                catch (Exception ex)
                {
                    exceptionAction(ex);
                }
                if (success)
                {  
                    yield return result;
                }
            }
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}
