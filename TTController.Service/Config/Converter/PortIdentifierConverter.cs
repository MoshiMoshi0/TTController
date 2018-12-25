using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public class PortIdentifierConverter : ObjectToArrayConverter<PortIdentifier>
    { 
        protected override object[] CreateConstructorArgs(JArray array) =>
            new object[]{ array[0].Value<int>(), array[1].Value<int>(), array[2].Value<byte>() };
    }
}
