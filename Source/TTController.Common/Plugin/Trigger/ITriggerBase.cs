using System;

namespace TTController.Common.Plugin
{
    public interface ITriggerBase : IPlugin, IDisposable
    {
        bool Value(ICacheProvider cache);
    }
}
