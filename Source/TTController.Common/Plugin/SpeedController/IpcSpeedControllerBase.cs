using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class IpcSpeedControllerBase<T> : SpeedControllerBase<T>, IIpcClient where T : SpeedControllerConfigBase
    {
        private readonly CancellationTokenSource _cancellationSource;
        private readonly Task _receiveTask;

        public abstract string IpcName { get; }
        public Channel<string> SendChannel { get; }
        public Channel<string> ReceiveChannel { get; }

        protected IpcSpeedControllerBase(T config) : base(config)
        {
            SendChannel = null;
            ReceiveChannel = Channel.CreateBounded<string>(8);

            _cancellationSource = new CancellationTokenSource();
            _receiveTask = Task.Factory.StartNew(() => ReceiveAsync(_cancellationSource.Token), _cancellationSource.Token);
        }

        protected abstract void OnDataReceived(string data);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _cancellationSource.Cancel();
            _receiveTask.Wait();
            _cancellationSource.Dispose();
        }

        private async void ReceiveAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await ReceiveChannel.Reader.ReadAsync(cancellationToken);
                    OnDataReceived(result);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
