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
    }
}
