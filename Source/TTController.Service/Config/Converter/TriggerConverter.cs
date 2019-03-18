using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public class TriggerConverter : AbstractPluginConverter<ITriggerBase, TriggerConfigBase> { }
}
