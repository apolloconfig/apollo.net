using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Core.Ioc;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    [Named(ServiceType = typeof(ConfigRegistry))]
    class DefaultConfigRegistry : ConfigRegistry
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultConfigRegistry));
        private IDictionary<string, ConfigFactory> m_instances = new ConcurrentDictionary<string, ConfigFactory>();

        public void Register(string namespaceName, ConfigFactory factory)
        {
            if (m_instances.ContainsKey(namespaceName))
            {
                logger.Warn(string.Format("ConfigFactory({0}) is overridden by {1}!", namespaceName, factory.GetType()));
            }

            m_instances[namespaceName] = factory;

        }

        public ConfigFactory GetFactory(string namespaceName)
        {
            ConfigFactory config;
            m_instances.TryGetValue(namespaceName, out config);
            return config;
        }
    }
}
