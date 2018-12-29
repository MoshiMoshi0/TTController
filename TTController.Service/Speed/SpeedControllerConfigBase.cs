using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Service.Trigger;

namespace TTController.Service.Speed
{
    public abstract class SpeedControllerConfigBase
    {
        public ITriggerBase Trigger { get; set; } = new AlwaysTrigger();
    }
}
