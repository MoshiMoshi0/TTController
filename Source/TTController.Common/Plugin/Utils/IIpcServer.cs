using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTController.Common.Plugin
{
    public interface IIpcServer : IPlugin, IDisposable
    {
        void Register(IIpcClient client);
        void Start();
    }
}
