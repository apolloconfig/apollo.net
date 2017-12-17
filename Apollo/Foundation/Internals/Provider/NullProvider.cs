using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    internal class NullProvider : IApplicationProvider, INetworkProvider, IServerProvider
    {

        public string AppId => null;

        public void Initialize(System.IO.Stream stream)
        {

        }

        public Type Type => null;

        public string Property(string name, string defaultValue)
        {
            return defaultValue;
        }

        public void Initialize()
        {

        }

        public string HostAddress => null;

        public string HostName => null;

        public string EnvType => null;

        public string SubEnvType => null;

        public string DataCenter => null;

        public bool AppIdSet => false;
    }
}
