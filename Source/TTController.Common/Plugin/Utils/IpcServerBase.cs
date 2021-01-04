using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class IpcServerBase<T> : IIpcServer where T : IpcServerConfigBase
    {
        public T Config { get; private set; }
        protected Dictionary<string, List<IIpcClient>> Clients { get; }

        protected IpcServerBase(T config)
        {
            Config = config;
            Clients = new Dictionary<string, List<IIpcClient>>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Config = null;
        }

        public virtual void Register(IIpcClient client)
        {
            if (!Clients.ContainsKey(client.IpcName))
                Clients.Add(client.IpcName, new List<IIpcClient>());

            Clients[client.IpcName].Add(client);
        }

        public abstract void Start();
    }
}
