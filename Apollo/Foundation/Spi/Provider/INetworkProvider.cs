using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface INetworkProvider : IProvider
    {
        string HostAddress { get; }
        string HostName { get; }
    }
}
