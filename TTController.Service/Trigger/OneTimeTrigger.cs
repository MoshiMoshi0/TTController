using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Service.Trigger
{
    public class OneTimeTrigger : TriggerBase
    {
        private bool _triggered;

        public OneTimeTrigger()
        {
            _triggered = false;
        }

        public override bool Value()
        {
            if (_triggered)
                return false;

            _triggered = true;
            return true;
        }
    }
}
