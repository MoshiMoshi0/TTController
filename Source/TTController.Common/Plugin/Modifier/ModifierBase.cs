using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class ModifierBase<T> : IModifierBase where T : ModifierConfigBase
    {
        protected T Config { get; private set; }

        protected ModifierBase(T config)
        {
            Config = config;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Config = null;
        }
    }
}
