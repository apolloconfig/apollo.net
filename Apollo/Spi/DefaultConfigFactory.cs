using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Internals;
using System.IO;
using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    [Named(ServiceType = typeof(ConfigFactory))]
    class DefaultConfigFactory : ConfigFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultConfigFactory));
        [Inject]
        private ConfigUtil m_configUtil;

        public Config Create(string namespaceName)
        {
            DefaultConfig defaultConfig = new DefaultConfig(namespaceName, CreateLocalConfigRepository(namespaceName));
            return defaultConfig;
        }

        LocalFileConfigRepository CreateLocalConfigRepository(string namespaceName)
        {
            if (m_configUtil.InLocalMode)
            {
                Console.WriteLine(
                    string.Format("==== Apollo is in local mode! Won't pull configs from remote server for namespace {0} ! ====", namespaceName));
                return new LocalFileConfigRepository(namespaceName);
            }
            return new LocalFileConfigRepository(namespaceName, CreateRemoteConfigRepository(namespaceName));
        }

        RemoteConfigRepository CreateRemoteConfigRepository(string namespaceName)
        {
            return new RemoteConfigRepository(namespaceName);
        }
    }
}
