using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public interface IIpcClient : IPlugin, IDisposable
    {
        string IpcName { get; }
    }

    public interface IIpcReaderClient : IIpcClient
    {
        bool TryWrite(string item);
        ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default);
        ValueTask WriteAsync(string item, CancellationToken cancellationToken = default);
    }

    public interface IIpcWriterClient : IIpcClient
    {
        bool TryRead(out string item);
        ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default);
        ValueTask<string> ReadAsync(CancellationToken cancellationToken = default);
    }
}
