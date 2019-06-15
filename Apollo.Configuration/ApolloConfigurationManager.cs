using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;
using JetBrains.Annotations;
using System;
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

        public static IConfigManager Manager => _manager ?? throw new InvalidOperationException("请在使用之前调用AddApollo");

        internal static void SetApolloOptions(ConfigRepositoryFactory factory) =>
            Interlocked.CompareExchange(ref _manager, new DefaultConfigManager(new DefaultConfigRegistry(), factory), null);

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public Task<IConfig> GetConfig([NotNull]string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName)) throw new ArgumentException("message", nameof(namespaceName));

            return _manager.GetConfig(namespaceName);
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public async Task<IConfig> GetConfig([NotNull]params string[] namespaces)
        {
            if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));

            return new MultiConfig(await Task.WhenAll(namespaces.Select(GetConfig)).ConfigureAwait(false));
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public async Task<IConfig> GetConfig([NotNull]IEnumerable<string> namespaces)
        {
            if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));

            return new MultiConfig(await Task.WhenAll(namespaces.Select(GetConfig)).ConfigureAwait(false));
        }
    }
}

