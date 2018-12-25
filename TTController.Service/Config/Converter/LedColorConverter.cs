using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    class LedColorConverter : ObjectToArrayConverter<LedColor> {
        protected override object[] CreateConstructorArgs(JArray array) =>
            new object[]{ array[0].Value<byte>(), array[1].Value<byte>(), array[2].Value<byte>() };
    }
}
