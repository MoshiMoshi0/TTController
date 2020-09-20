using System;
using System.Threading.Channels;

namespace TTController.Common.Plugin
{
    public interface IIpcClient : IPlugin, IDisposable
    {
        string IpcName { get; }
        Channel<string> SendChannel { get; }
        Channel<string> ReceiveChannel { get; }
    }
}
