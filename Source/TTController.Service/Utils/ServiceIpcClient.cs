using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Service.Utils
{
    public class ServiceIpcClient : IIpcClient
    {
        private readonly Task _sendTask;
        private readonly  CancellationTokenSource _cancellationSource;
        private readonly EventWaitHandle _dirtyWaitHandle;
        private readonly List<ServiceIpcDataItem> _ipcData;

        public string IpcName => "service";

        public Channel<string> SendChannel { get; }
        public Channel<string> ReceiveChannel { get; }

        public ServiceIpcClient()
        {
            ReceiveChannel = null;
            SendChannel = Channel.CreateBounded<string>(1);

            _cancellationSource = new CancellationTokenSource();
            _sendTask = Task.Factory.StartNew(() => SendAsync(_cancellationSource.Token), _cancellationSource.Token);
            _dirtyWaitHandle = new AutoResetEvent(false);
            _ipcData = new List<ServiceIpcDataItem>();
        }

        private async void SendAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await _dirtyWaitHandle.WaitOneAsync(cancellationToken);
                    await SendChannel.Writer.WaitToWriteAsync(cancellationToken);

                    var data = JsonConvert.SerializeObject(_ipcData, Formatting.None);
                    await SendChannel.Writer.WriteAsync(data, cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Update(IEnumerable<PortIdentifier> ports, ICacheProvider cache)
        {
            _ipcData.Clear();
            foreach (var port in ports)
            {
                _ipcData.Add(new ServiceIpcDataItem()
                {
                    Port = port,
                    Data = cache.GetPortData(port),
                    Colors = cache.GetPortColors(port),
                    Speed = cache.GetPortSpeed(port),
                    DeviceConfig = cache.GetDeviceConfig(port)
                });
            }

            _dirtyWaitHandle.Set();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _cancellationSource.Cancel();
            _sendTask.Wait();
            _cancellationSource.Dispose();
            _dirtyWaitHandle.Dispose();
        }

        private class ServiceIpcDataItem
        {
            public PortIdentifier Port { get; set; }
            public List<LedColor> Colors { get; set; }
            public byte? Speed { get; set; }
            public PortData Data { get; set; }
            public DeviceConfig DeviceConfig { get; set; }
        }
    }
}
