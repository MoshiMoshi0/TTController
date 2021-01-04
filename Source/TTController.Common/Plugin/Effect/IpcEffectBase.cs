using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class IpcEffectBase<T> : EffectBase<T>, IIpcReaderClient where T : EffectConfigBase
    {
        private readonly CancellationTokenSource _cancellationSource;
        private readonly Task _receiveTask;
        private readonly Channel<string> _channel;

        public abstract string IpcName { get; }

        protected IpcEffectBase(T config) : base(config)
        {
            _channel = Channel.CreateBounded<string>(8);
            _cancellationSource = new CancellationTokenSource();
            _receiveTask = Task.Factory.StartNew(() => ReceiveAsync(_cancellationSource.Token), _cancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        protected abstract void OnDataReceived(string data);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _cancellationSource.Cancel();
            _receiveTask.Wait();
            _cancellationSource.Dispose();
        }

        private async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await _channel.Reader.ReadAsync(cancellationToken);
                    OnDataReceived(result);
                }
            }
            catch (OperationCanceledException) { }
        }

        public bool TryWrite(string item) => _channel.Writer.TryWrite(item);
        public ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default) => _channel.Writer.WaitToWriteAsync(cancellationToken);
        public ValueTask WriteAsync(string item, CancellationToken cancellationToken = default) => _channel.Writer.WriteAsync(item, cancellationToken);
    }
}
