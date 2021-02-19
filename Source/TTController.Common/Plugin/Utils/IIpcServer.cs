using System;
using System.Threading;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public interface IIpcServer : IPlugin, IDisposable
    {
        void Register(IIpcClient client);
        void Start();
    }

    public interface IIpcClient : IPlugin, IDisposable
    {
        string IpcName { get; }
    }

    public interface IIpcReader
    {
        ValueTask WriteAsync(string item, CancellationToken cancellationToken = default);
    }

    public interface IIpcWriter
    {
        ValueTask<string> ReadAsync(CancellationToken cancellationToken = default);
    }
}
