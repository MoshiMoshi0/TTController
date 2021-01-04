using System;

namespace TTController.Common.Plugin
{
    public interface IIpcServer : IPlugin, IDisposable
    {
        void Register(IIpcClient client);
        void Start();
    }
}
