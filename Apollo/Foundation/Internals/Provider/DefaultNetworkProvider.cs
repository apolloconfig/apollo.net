using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;

namespace Com.Ctrip.Framework.Foundation.Internals.Provider
{
    class DefaultNetworkProvider : INetworkProvider
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultNetworkProvider));

        public string HostAddress
        {
            get { return NetworkInterfaceManager.HostIP; }
        }

        public string HostName
        {
            get { return NetworkInterfaceManager.HostName; }
        }

        public Type Type
        {
            get { return typeof(INetworkProvider); }
        }

        public string Property(string name, string defaultValue)
        {
            name = (name ?? string.Empty).ToLower();
            if ("host.address" == name)
            {
                return HostAddress ?? defaultValue;
            }
            else if ("host.name" == name)
            {
                return HostName ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        void IProvider.Initialize()
        {
            try { 
                NetworkInterfaceManager.Refresh();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
