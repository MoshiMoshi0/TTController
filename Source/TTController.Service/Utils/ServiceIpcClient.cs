using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Service.Utils
{
    public class ServiceIpcClient : IIpcClient, IIpcWriter, IIpcReader
    {
        private readonly List<PortIdentifier> _ports;
        private readonly ICacheProvider _cache;

        private readonly Task _receiveTask;
        private readonly ConcurrentDictionary<DataType, (Task Task, CancellationTokenSource CancellationSource)> _sendTasks;

        private readonly CancellationTokenSource _cancellationSource;
        private readonly Channel<string> _readerChannel;
        private readonly Channel<string> _writerChannel;

        public string IpcName => "service";

        public ServiceIpcClient(IEnumerable<PortIdentifier> ports, ICacheProvider cache)
        {
            _ports = ports.ToList();
            _cache = cache;

            _readerChannel = Channel.CreateBounded<string>(1);
            _writerChannel = Channel.CreateBounded<string>(1);

            _cancellationSource = new CancellationTokenSource();

            _receiveTask = Task.Factory.StartNew(() => ReceiveAsync(_cancellationSource.Token), _cancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
            _sendTasks = new ConcurrentDictionary<DataType, (Task Task, CancellationTokenSource CancellationSource)>();
        }

        private async Task SendAsync(CancellationToken cancellationToken, DataType type, int interval)
        {
            try
            {
                var settings = JsonConvert.DefaultSettings();
                settings.NullValueHandling = NullValueHandling.Include;
                settings.DefaultValueHandling = DefaultValueHandling.Include;
                settings.Formatting = Formatting.None;

                while (!cancellationToken.IsCancellationRequested)
                {
                    var data = default(string);
                    switch (type)
                    {
                        case DataType.Colors:
                            data = JsonConvert.SerializeObject(_ports.Select(p => new { Port = p, Colors = _cache.GetPortColors(p) }), settings);
                            break;
                        case DataType.Speed:
                            data = JsonConvert.SerializeObject(_ports.Select(p => new { Port = p, Speed = _cache.GetPortSpeed(p) }), settings);
                            break;
                        case DataType.Data:
                            data = JsonConvert.SerializeObject(_ports.Select(p => new { Port = p, Data = _cache.GetPortData(p) }), settings);
                            break;
                        case DataType.Config:
                            data = JsonConvert.SerializeObject(_ports.Select(p => new { Port = p, Config = _cache.GetPortConfig(p) }), settings);
                            break;
                    }

                    await _writerChannel.Writer.WriteAsync(data, cancellationToken);
                    await Task.Delay(interval);
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await _readerChannel.Reader.ReadAsync(cancellationToken);

                    if (string.IsNullOrWhiteSpace(result))
                        continue;

                    try
                    {
                        var document = JObject.Parse(result);

                        if (document.TryGetValue("Enable", StringComparison.OrdinalIgnoreCase, out var enableToken)
                         && document.TryGetValue("Interval", StringComparison.OrdinalIgnoreCase, out var intervalToken)
                         && Enum.TryParse<DataType>(enableToken.Value<string>(), out var enableType))
                        {
                            Enable(enableType, intervalToken.Value<int>());
                        }
                        else if (document.TryGetValue("Disable", StringComparison.OrdinalIgnoreCase, out var disableToken)
                              && Enum.TryParse<DataType>(disableToken.Value<string>(), out var disableType))
                        {
                            Disable(disableType);
                        }
                    }
                    catch (JsonReaderException) { }
                }
            }
            catch (OperationCanceledException) { }
        }

        private void Enable(DataType type, int interval)
        {
            if (_sendTasks.ContainsKey(type))
                return;

            var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationSource.Token);
            var task = Task.Factory.StartNew(() => SendAsync(cancellationSource.Token, type, interval), _cancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();

            _sendTasks.TryAdd(type, (task, cancellationSource));
        }

        private void Disable(DataType type)
        {
            if (!_sendTasks.ContainsKey(type))
                return;

            var (task, cancellationSource) = _sendTasks[type];

            cancellationSource.Cancel();
            task.Wait();
            cancellationSource.Dispose();

            _sendTasks.TryRemove(type, out var _);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _cancellationSource.Cancel();
            Task.WaitAll(_receiveTask);

            foreach (var (task, cancellationSource) in _sendTasks.Values)
            {
                cancellationSource.Cancel();
                task.Wait();
                cancellationSource.Dispose();
            }

            _cancellationSource.Dispose();
        }

        public ValueTask<string> ReadAsync(CancellationToken cancellationToken = default) => _writerChannel.Reader.ReadAsync(cancellationToken);
        public ValueTask WriteAsync(string item, CancellationToken cancellationToken = default) => _readerChannel.Writer.WriteAsync(item, cancellationToken);

        private enum DataType
        {
            Colors,
            Speed,
            Data,
            Config
        }
    }
}
