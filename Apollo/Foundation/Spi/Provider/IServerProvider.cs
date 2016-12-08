using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface IServerProvider : IProvider
    {
        string EnvType { get; }
        string SubEnvType { get;  }
        string DataCenter { get; }
        void Initialize(Stream stream);
    }
}
