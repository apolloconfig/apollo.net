using Com.Ctrip.Framework.Foundation.Internals.Provider;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;

namespace Com.Ctrip.Framework.Foundation.Internals
{
    class DefaultProviderManager : IProviderManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultProviderManager));
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
            return NullProviderManager.provider;
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
