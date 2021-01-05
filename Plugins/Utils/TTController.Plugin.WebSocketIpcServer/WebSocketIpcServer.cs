using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Service.Ipc
{
    public class WebSocketIpcServerConfig : IpcServerConfigBase
    {
        [DefaultValue("127.0.0.1")] public string Address { get; internal set; } = "127.0.0.1";
        [DefaultValue(8888)] public short Port { get; internal set; } = 8888;
    }

    public class WebSocketIpcServer : IpcServerBase<WebSocketIpcServerConfig>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPAddress _address;
        private readonly int _port;
        private readonly HttpListener _listener;
        private readonly List<Task> _tasks;
        private readonly CancellationTokenSource _cancellationSource;

        public WebSocketIpcServer(WebSocketIpcServerConfig config) : base(config)
        {
            _address = IPAddress.Parse(config.Address);
            _port = config.Port;

            _listener = new HttpListener();
            _tasks = new List<Task>();
            _cancellationSource = new CancellationTokenSource();
        }

        public override void Register(IIpcClient client)
        {
            base.Register(client);

            var uri = $"{_address}:{_port}/{client.IpcName}/";
            if (!_listener.Prefixes.Contains(uri, StringComparer.OrdinalIgnoreCase))
            {
                Logger.Info("Registered new websocket url: \"ws://{0}\"", uri);
                _listener.Prefixes.Add($"http://{uri}");
            }
        }

        public override void Start()
            => _tasks.Add(Task.Factory.StartNew(() => StartAsync(_cancellationSource.Token), _cancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());

        private async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.Info("Starting websocket server on ws://{0}:{1}", Config.Address, Config.Port);
                _listener.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    var context = await _listener.GetContextAsync().WithCancellation(cancellationToken);
                    if (!context.Request.IsWebSocketRequest)
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        continue;
                    }

                    try
                    {
                        var webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null).WithCancellation(cancellationToken);

                        var ipcName = webSocketContext.RequestUri.Segments.Last();
                        if (!Clients.TryGetValue(ipcName, out var clients) || clients.Count == 0)
                            continue;

                        Logger.Info("New websocket connection: {1}", ipcName, webSocketContext.RequestUri);
                        _tasks.Add(Task.Factory.StartNew(() => ReceiveAsync(ipcName, webSocketContext, cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                        _tasks.Add(Task.Factory.StartNew(() => SendAsync(ipcName, webSocketContext, cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap());
                    }
                    catch
                    {
                        context.Response.StatusCode = 500;
                        context.Response.Close();
                        continue;
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
            catch (WebSocketException) { }
        }

        private async Task ReceiveAsync(string ipcName, WebSocketContext context, CancellationToken cancellationToken)
        {
            try
            {
                var socket = context.WebSocket;

                try
                {
                    if (!Clients.ContainsKey(ipcName))
                        return;

                    var clients = Clients[ipcName].OfType<IIpcReader>().ToList();
                    if (!clients.Any())
                        return;

                    while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
                    {
                        var result = await ReceiveStringAsync(socket, cancellationToken);
                        if (string.IsNullOrWhiteSpace(result))
                            continue;

                        Logger.Debug("\"{0}\" received data: \"{1}\"", ipcName, result);
                        foreach (var client in clients)
                            await client.WriteAsync(result, cancellationToken);
                    }
                }
                catch (OperationCanceledException) { }

                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch (WebSocketException) { }
        }

        private async Task SendAsync(string ipcName, WebSocketContext context, CancellationToken cancellationToken)
        {
            try
            {
                var socket = context.WebSocket;

                try
                {
                    if (!Clients.ContainsKey(ipcName))
                        return;

                    var clients = Clients[ipcName].OfType<IIpcWriter>().ToList();
                    if (!clients.Any())
                        return;

                    var semaphore = new SemaphoreSlim(1, 1);
                    await Task.WhenAll(clients.Select(client =>
                        Task.Factory.StartNew(async () =>
                        {
                            while (!cancellationToken.IsCancellationRequested && socket.State == WebSocketState.Open)
                            {
                                var result = await client.ReadAsync(cancellationToken);
                                Logger.Debug("\"{0}\" sent data: \"{1}\"", ipcName, result);
                                if (string.IsNullOrWhiteSpace(result))
                                    continue;

                                await semaphore.WaitAsync(cancellationToken);
                                await SendStringAsync(socket, result, cancellationToken);
                                semaphore.Release();
                            }
                        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap()
                    ));
                }
                catch (OperationCanceledException) { }

                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch (WebSocketException) { }
        }

        private async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken cancellationToken)
        {
            var result = default(WebSocketReceiveResult);
            var buffer = new ArraySegment<byte>(new byte[1024]);
            using (var stream = new MemoryStream())
            {
                do
                {
                    result = await socket.ReceiveAsync(buffer, cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
                        return null;
                    }

                    await stream.WriteAsync(buffer.Array, buffer.Offset, result.Count, cancellationToken);
                }
                while (!result.EndOfMessage);

                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        private async Task SendStringAsync(WebSocket socket, string data, CancellationToken cancellationToken)
            => await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)), WebSocketMessageType.Text, endOfMessage: true, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _cancellationSource.Cancel();
            Task.WaitAll(_tasks.ToArray());
            _cancellationSource.Dispose();
            _tasks.Clear();

            _listener.Stop();
            _listener.Close();
        }
    }
}
