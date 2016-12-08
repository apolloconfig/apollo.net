using System;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Spi;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    [Named(ServiceType = typeof(ConfigManager))]
    public class DefaultConfigManager : ConfigManager
    {
        [Inject]
        private ConfigFactoryManager m_factoryManager;
        private IDictionary<string, Config> m_configs = new ConcurrentDictionary<string, Config>();

        public Config GetConfig(String namespaceName) {
            Config config;
            m_configs.TryGetValue(namespaceName, out config);

            if (config == null)
            {
                lock (this)
                {
                    m_configs.TryGetValue(namespaceName, out config);

                    if (config == null)
                    {
                        ConfigFactory factory = m_factoryManager.GetFactory(namespaceName);

                        config = factory.Create(namespaceName);
                        m_configs[namespaceName] = config;
                    }
                }
            }

            return config;

        }
    }
}

