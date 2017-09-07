using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    class NullProvider : IApplicationProvider, INetworkProvider, IServerProvider
    {

        public string AppId
        {
            get { return null; }
        }

        public void Initialize(System.IO.Stream stream)
        {
            
        }

        public Type Type
        {
            get { return null; }
        }

        public string Property(string name, string defaultValue)
        {
            return defaultValue;
        }

        public void Initialize()
        {
            
        }

        public string HostAddress
        {
            get { return null; }
        }

        public string HostName
        {
            get { return null; }
        }

        public string EnvType
        {
            get { return null; }
        }

        public string SubEnvType
        {
            get { return null; }
        }

        public string DataCenter
        {
            get { return null; }
        }

        public bool AppIdSet
        {
            get { return false; }
        }
    }
}
