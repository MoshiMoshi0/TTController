using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converters
{
    public class LedColorConverter : AbstractObjectToArrayConverter<LedColor>
    {
        protected override object[] CreateConstructorArgs(JArray array) =>
            new object[] { array[0].Value<byte>(), array[1].Value<byte>(), array[2].Value<byte>() };
    }
}
