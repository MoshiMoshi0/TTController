using System;
using System.Collections.Generic;
using System.Linq;

namespace TTController.Service.Utils
{
    public static class TypeUtils
    {
        public static IEnumerable<Type> FindInAssemblies<TBase>() where TBase : class
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract);

            if (typeof(TBase).IsInterface)
                return types.Where(t => typeof(TBase).IsAssignableFrom(t));
            else
                return types.Where(t => t.IsSubclassOf(typeof(TBase)));
        }
    }
}
