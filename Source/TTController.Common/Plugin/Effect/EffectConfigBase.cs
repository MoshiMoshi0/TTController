using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LibreHardwareMonitor.Hardware;

namespace TTController.Common.Plugin
{
    public abstract class EffectConfigBase
    {
        [DefaultValue(null)] public ITriggerBase Trigger { get; private set; } = null;
        [DefaultValue(ColorGenerationMethod.PerPort)] public ColorGenerationMethod ColorGenerationMethod { get; private set; } = ColorGenerationMethod.PerPort;
    }
}
