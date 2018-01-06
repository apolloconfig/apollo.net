using System;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Foundation.Internals;
using Com.Ctrip.Framework.Foundation.Spi.Provider;

namespace Com.Ctrip.Framework.Apollo.Foundation.Internals.Provider
{
    internal class DefaultNetworkProvider : INetworkProvider
    {
        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(DefaultNetworkProvider));

        public string HostAddress => NetworkInterfaceManager.HostIp;

        public string HostName => NetworkInterfaceManager.HostName;

        public Type Type => typeof(INetworkProvider);

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
            try
            {
                NetworkInterfaceManager.Refresh();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
