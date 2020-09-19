using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public interface IIpcServer : IPlugin, IDisposable
    {
        void RegisterClient(IIpcClient client);
        void Start();
    }
}
