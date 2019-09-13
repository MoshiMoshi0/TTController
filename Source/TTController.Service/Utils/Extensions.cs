using System;
using System.Collections.Generic;
using System.Linq;

namespace TTController.Service.Utils
{
    public static class Extensions
    {
        public static IEnumerable<Type> FindImplementations(this Type type)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract);

            if (type.IsInterface)
                return types.Where(t => type.IsAssignableFrom(t));
            else
                return types.Where(t => t.IsSubclassOf(type));
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static void Deconstruct<TKey, TElement>(this IGrouping<TKey, TElement> group, out TKey key, out IEnumerable<TElement> elements)
        {
            key = group.Key;
            elements = group.AsEnumerable();
        }

        public static IEnumerable<T> RotateLeft<T>(this IEnumerable<T> enumberable, int rotate) 
            => enumberable.Skip(rotate).Concat(enumberable.Take(rotate));

        public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> enumberable, int rotate)
            => enumberable.RotateLeft(enumberable.Count() - rotate);
    }
}
