using LibreHardwareMonitor.Hardware;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class IpcTriggerBase<T> : TriggerBase<T>, IIpcClient where T : TriggerConfigBase
    {
        private Task _receiveTask;
        private CancellationTokenSource _cancellationSource;

        public abstract string IpcName { get; }
        public Channel<string> SendChannel { get; private set; }
        public Channel<string> ReceiveChannel { get; private set; }

        protected IpcTriggerBase(T config) : base(config)
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
