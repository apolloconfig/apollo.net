using System;
using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo.Foundation.Internals.Provider;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Foundation.Internals;
using Com.Ctrip.Framework.Foundation.Internals.Provider;
using Com.Ctrip.Framework.Foundation.Spi.Provider;

namespace Com.Ctrip.Framework.Apollo.Foundation.Internals
{
    class DefaultProviderManager : IProviderManager
    {
        private static readonly ILogger logger = LogManager.CreateLogger(typeof(DefaultProviderManager));
        private readonly object syncLock = new object();
        private IDictionary<Type, IProvider> providers = new Dictionary<Type, IProvider>();

        public DefaultProviderManager()
        {
            IApplicationProvider applicationProvider = new DefaultApplicationProvider();
            applicationProvider.Initialize();
            Register(applicationProvider);

            logger.Info(applicationProvider.ToString());

            IProvider networkProvider = new DefaultNetworkProvider();
            networkProvider.Initialize();
            Register(networkProvider);

            IProvider serverProvider = new DefaultServerProvider();
            serverProvider.Initialize();
            Register(serverProvider);
        }

        public string GetProperty(string name, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public IProvider Provider(Type clazz)
        {
            IProvider provider;
            providers.TryGetValue(clazz, out provider);
            if (null != provider)
            {
                return provider;
            }
            logger.Info("No provider is found in DefaultProviderManager for type: " + clazz);
            return NullProviderManager.Provider;
        }

        private void Register(IProvider p)
        {
            lock (syncLock)
            {
                providers[p.Type] = p;
            }
        }
    }
}
