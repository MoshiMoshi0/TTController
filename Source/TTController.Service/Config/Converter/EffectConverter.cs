using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTController.Common;
using TTController.Service.Config.Data;
using TTController.Service.Utils;

namespace TTController.Service.Config.Converter
{
    public class EffectConverter : AbstractPluginConverter<IEffectBase, EffectConfigBase> { }
}
