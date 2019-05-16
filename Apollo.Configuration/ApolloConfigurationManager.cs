using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
    public class ApolloConfigurationManager
    {
        private static IConfigManager _manager;

        internal static void SetApolloOptions(ConfigRepositoryFactory factory) =>
            Interlocked.CompareExchange(ref _manager, new DefaultConfigManager(new DefaultConfigFactoryManager(new DefaultConfigRegistry(), factory)), null);

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public Task<IConfig> GetConfig(string namespaceName) => _manager.GetConfig(namespaceName);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public async Task<IConfig> GetConfig(IEnumerable<string> namespaces) =>
            new MultiConfig(await Task.WhenAll(namespaces.Select(GetConfig)).ConfigureAwait(false));
    }
}

