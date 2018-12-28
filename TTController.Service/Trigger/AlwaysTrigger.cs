using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTController.Service.Rgb;

namespace TTController.Service.Trigger
{
    public class AlwaysTrigger : TriggerBase
    {
        public override bool Value() => true;
    }
}
