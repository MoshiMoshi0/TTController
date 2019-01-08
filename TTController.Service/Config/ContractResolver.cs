using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace TTController.Service.Config
{
    public class ContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.GetInterfaces().Any(i => 
                i.IsGenericType 
                 && i.GetGenericTypeDefinition() == typeof(IDictionary<,>) 
                 && i.GetGenericArguments()[0] != typeof(string)
                ))
            {
                return base.CreateArrayContract(objectType);
            }

            return base.CreateContract(objectType);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.Writable)
                return property;

            property.Writable = (member as PropertyInfo)?.SetMethod != null;
            return property;
        }
    }
}
