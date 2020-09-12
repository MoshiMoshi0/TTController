using System.Threading.Channels;

namespace TTController.Common.Plugin
{
    public interface IIpcClient : IPlugin
    {
        string IpcName { get; }
        Channel<string> SendChannel { get; }
        Channel<string> ReceiveChannel { get; }
    }
}
