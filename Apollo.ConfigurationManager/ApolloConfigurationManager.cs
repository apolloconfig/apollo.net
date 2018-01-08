using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
    public static class ApolloConfigurationManager
    {
        private static readonly IConfigManager Manager = new DefaultConfigManager(
            new DefaultConfigFactoryManager(new DefaultConfigRegistry()));

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public static IConfig GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public static IConfig GetConfig(string namespaceName) => Manager.GetConfig(namespaceName);
    }
}

