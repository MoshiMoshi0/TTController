using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converters
{
    public class LedColorGradientPointConverter : AbstractObjectToArrayConverter<LedColorGradientPoint>
    {
        protected override object[] CreateConstructorArgs(JArray array) =>
            new object[] { array[0].Value<double>(), array[1].ToObject<LedColor>() };
    }
}
