using System;
using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using System.Collections.Concurrent;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    [Named(ServiceType = typeof(ConfigFactoryManager))]
    public class DefaultConfigFactoryManager : ConfigFactoryManager
    {
        [Inject]
        private ConfigRegistry m_registry;

        private IDictionary<string, ConfigFactory> m_factories = new ConcurrentDictionary<string, ConfigFactory>();

        public ConfigFactory GetFactory(String namespaceName) {
            // step 1: check hacked factory
            ConfigFactory factory = m_registry.GetFactory(namespaceName);

            if (factory != null)
            {
                return factory;
            }

            // step 2: check cache
            m_factories.TryGetValue(namespaceName, out factory);

            if (factory != null)
            {
                return factory;
            }

            // step 3: check declared config factory
            try
            {
                factory = ComponentLocator.Lookup<ConfigFactory>(namespaceName);
            }
            catch (Exception)
            {
                // ignore it
            }

            // step 4: check default config factory
            if (factory == null)
            {
                factory = ComponentLocator.Lookup<ConfigFactory>();
            }

            m_factories[namespaceName] = factory;

            // factory should not be null
            return factory;
        }
    }
}

