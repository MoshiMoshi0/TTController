using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
