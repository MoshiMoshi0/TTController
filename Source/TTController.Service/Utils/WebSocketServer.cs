using NLog;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using TTController.Common.Plugin;

namespace TTController.Service.Utils
{
    public class WebSocketServer : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPAddress _address;
        private readonly int _port;
        private readonly HttpListener _listener;
        private readonly List<Task> _tasks;
        private readonly List<IIpcClient> _clients;
        private readonly CancellationTokenSource _cancellationSource;

        public WebSocketServer(IPAddress address, int port)
        {
            _address = address;
            _port = port;

            _listener = new HttpListener();
            _tasks = new List<Task>();
            _clients = new List<IIpcClient>();
            _cancellationSource = new CancellationTokenSource();
        }

        public void RegisterClient(IIpcClient client)
        {
            Logger.Info("Registered IPC client: \"{0}\"", client.IpcName);
            _clients.Add(client);
            _listener.Prefixes.Add($"http://{_address}:{_port}/{client.IpcName}/");
        }

        public void Start()
            => _tasks.Add(Task.Factory.StartNew(() => StartAsync(_cancellationSource.Token), _cancellationSource.Token));

        private async void StartAsync(CancellationToken cancellationToken)
        {
            try
            {
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
                        var client = _clients.FirstOrDefault(c => webSocketContext.RequestUri.Segments[1].StartsWith(c.IpcName));

                        if (client == null)
                            continue;

                        Logger.Info("New IPC connection: {1}", client.IpcName, webSocketContext.RequestUri);
                        if(client.ReceiveChannel != null)
                            _tasks.Add(Task.Factory.StartNew(() => ReceiveTask(webSocketContext, client, cancellationToken), cancellationToken));

                        if(client.SendChannel != null)
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

        protected virtual void Dispose(bool disposing)
        {
            _cancellationSource.Cancel();
            Task.WaitAll(_tasks.ToArray());
            _cancellationSource.Dispose();
            _tasks.Clear();

            _listener.Stop();
            _listener.Close();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
