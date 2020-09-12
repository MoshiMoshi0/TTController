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

            return types.Where(t => type.IsAssignableOrSubclass(t));
        }

        public static bool IsAssignableOrSubclass(this Type type, Type c)
            => type.IsInterface
                ? type.IsAssignableFrom(c)
                : c.IsSubclassOf(type) || type == c;
    }
}
