using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TTController.Service.Config
{
    public class ContractResolver : DefaultContractResolver
    {
        private readonly SerializationCallback OnDeserializedCallback = (o, context) =>
        {
            if (context.Context is TrackingSerializationContext tracker)
                tracker.Handle(o);
        };

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.Writable)
                return property;

            property.Writable = (member as PropertyInfo)?.SetMethod != null;
            return property;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            if (!contract.OnDeserializedCallbacks.Contains(OnDeserializedCallback))
                contract.OnDeserializedCallbacks.Add(OnDeserializedCallback);

            return contract;
        }
    }
}
