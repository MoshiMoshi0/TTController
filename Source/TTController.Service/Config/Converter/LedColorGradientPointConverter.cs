using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    class LedColorGradientPointConverter : ObjectToArrayConverter<LedColorGradientPoint>
    {
        protected override object[] CreateConstructorArgs(JArray array) =>
            new object[] { array[0].Value<double>(), array[1].ToObject<LedColor>() };
    }
}
