using System;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.VenusBuild;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
    public class ConfigService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ConfigService));
        private static ConfigManager s_configManager;
        
        static ConfigService() {
            try
            {
                ComponentsConfigurator.DefineComponents();
                s_configManager = ComponentLocator.Lookup<ConfigManager>();
            }
            catch (Exception ex)
            {
                ApolloConfigException exception = new ApolloConfigException("Init ConfigService failed", ex);
                logger.Error(exception);
                throw exception;
            }
        }

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public static Config GetAppConfig() {
            return GetConfig(ConfigConsts.NAMESPACE_APPLICATION);
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public static Config GetConfig(String namespaceName) {
            return s_configManager.GetConfig(namespaceName);
        }
    }
}

