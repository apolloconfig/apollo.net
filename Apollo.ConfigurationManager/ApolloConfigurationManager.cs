using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;
using Com.Ctrip.Framework.Apollo.Util;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
#if NET471
    [Obsolete("不建议使用，推荐使用System.Configuration.ConfigurationBuilder + System.Configuration.ConfigurationManager")]
#elif NETSTANDARD
    [Obsolete("不建议使用，推荐安装包Com.Ctrip.Framework.Apollo.Configuration")]
#endif
    public static class ApolloConfigurationManager
    {
        public static IConfigManager Manager { get; } = new DefaultConfigManager(new DefaultConfigRegistry(), new ConfigRepositoryFactory(new ConfigUtil()));

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetConfig([NotNull]string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName)) throw new ArgumentNullException(nameof(namespaceName));

            return Manager.GetConfig(namespaceName);
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetConfig([NotNull] params string[] namespaces) => GetConfig((IEnumerable<string>)namespaces);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public static async Task<IConfig> GetConfig([NotNull]IEnumerable<string> namespaces)
        {
            if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));

            return new MultiConfig(await Task.WhenAll(namespaces.Reverse().Distinct().Select(GetConfig)).ConfigureAwait(false));
        }
    }
}

