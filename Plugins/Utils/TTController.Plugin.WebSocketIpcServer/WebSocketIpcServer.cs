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

        public override void RegisterClient(IIpcClient client)
        {
            base.RegisterClient(client);

            var uri = $"{_address}:{_port}/{client.IpcName}/";
            if (!_listener.Prefixes.Contains(uri, StringComparer.OrdinalIgnoreCase))
            {
                Logger.Info("Registered new websocket url: \"ws://{0}\"", uri);
                _listener.Prefixes.Add($"http://{uri}");
            }
        }

        public override void Start()
            => _tasks.Add(Task.Factory.StartNew(() => StartAsync(_cancellationSource.Token), _cancellationSource.Token));

        private async void StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Logger.Info("Starting websocket server on ws://{0}:{1}", Config.Address, Config.Port);
                _listener.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    var context = await _listener.GetContextAsync();
                    if (!context.Request.IsWebSocketRequest)
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        continue;
                    }

                    try
                    {
                        var webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        var client = Clients.FirstOrDefault(c => webSocketContext.RequestUri.Segments[1].StartsWith(c.IpcName));

                        if (client == null)
                            continue;

                        Logger.Info("New websocket connection: {1}", client.IpcName, webSocketContext.RequestUri);
                        if (client.ReceiveChannel != null)
                            _tasks.Add(Task.Factory.StartNew(() => ReceiveTask(webSocketContext, client, cancellationToken), cancellationToken));

                        if (client.SendChannel != null)
                            _tasks.Add(Task.Factory.StartNew(() => SendTask(webSocketContext, client, cancellationToken), cancellationToken));
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
        }

        private async void ReceiveTask(WebSocketContext context, IIpcClient client, CancellationToken cancellationToken)
        {
            try
            {
                var webSocket = context.WebSocket;
                while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
                {
                    var result = await ReceiveStringAsync(webSocket, cancellationToken);

                    Logger.Debug("\"{0}\" received data: \"{1}\"", client.IpcName, result);
                    if (string.IsNullOrWhiteSpace(result))
                        continue;

                    await client.ReceiveChannel.Writer.WriteAsync(result, cancellationToken);
                    if (webSocket.State != WebSocketState.Open)
                        break;
                }
            }
            catch (OperationCanceledException) { }
        }

        private async void SendTask(WebSocketContext context, IIpcClient client, CancellationToken cancellationToken)
        {
            try
            {
                var webSocket = context.WebSocket;
                while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
                {
                    var result = await client.SendChannel.Reader.ReadAsync(cancellationToken);
                    if (webSocket.State != WebSocketState.Open)
                        break;

                    Logger.Debug("\"{0}\" sent data: \"{1}\"", client.IpcName, result);
                    if (string.IsNullOrWhiteSpace(result))
                        continue;

                    await SendStringAsync(webSocket, result, cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result;
            var buffer = new ArraySegment<byte>(new byte[1024]);
            using (var stream = new MemoryStream())
            {
                do
                {
                    result = await socket.ReceiveAsync(buffer, cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                        return null;

                    stream.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
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
