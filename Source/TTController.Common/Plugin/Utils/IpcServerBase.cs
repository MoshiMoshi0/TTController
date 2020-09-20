using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public abstract class IpcServerBase<T> : IIpcServer where T : IpcServerConfigBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public T Config { get; private set; }
        protected List<IIpcClient> Clients { get; private set; }

        protected IpcServerBase(T config)
        {
            Config = config;
            Clients = new List<IIpcClient>();
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

        public virtual void RegisterClient(IIpcClient client)
        {
            Logger.Info("Registered IPC client: \"{0}\"", client.IpcName);
            Clients.Add(client);
        }

        public abstract void Start();
    }
}
