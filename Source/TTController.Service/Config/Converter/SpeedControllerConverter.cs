using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;

namespace TTController.Service.Config.Converter
{
    public class SpeedControllerConverter : AbstractPluginConverter<ISpeedControllerBase, SpeedControllerConfigBase> { }
}
