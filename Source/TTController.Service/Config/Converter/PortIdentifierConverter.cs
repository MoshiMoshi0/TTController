using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public class PortIdentifierConverter : AbstractObjectToArrayConverter<PortIdentifier>
    {
        protected override object[] CreateConstructorArgs(JArray array) =>
            new object[] { array[0].Value<int>(), array[1].Value<int>(), array.Count == 2 ? (byte)0 : array[2].Value<byte>() };

        protected override IEnumerable<PropertyInfo> FilterProperties(PortIdentifier value)
        {
            foreach (var property in value.GetType().GetProperties())
            {
                if (string.CompareOrdinal(property.Name, nameof(PortIdentifier.Id)) != 0 || (byte)property.GetValue(value) > 0)
                    yield return property;
            }
        }
    }
}
