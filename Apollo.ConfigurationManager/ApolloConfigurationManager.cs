using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Spi;
using Com.Ctrip.Framework.Apollo.Util;
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
        private static readonly Exception? Exception;
        public static IConfigManager? Manager { get; }

        static ApolloConfigurationManager()
        {
            try
            {
                Manager = new DefaultConfigManager(new DefaultConfigRegistry(), new ConfigRepositoryFactory(new ConfigUtil()));
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetConfig(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName)) throw new ArgumentNullException(nameof(namespaceName));

            if (Exception != null) throw new InvalidOperationException("Apollo初始化异常", Exception);

            return Manager!.GetConfig(namespaceName);
        }

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public static Task<IConfig> GetConfig(params string[] namespaces) => GetConfig((IEnumerable<string>)namespaces);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaces"> the namespaces of the config, order desc. </param>
        /// <returns> config instance </returns>
        public static async Task<IConfig> GetConfig(IEnumerable<string> namespaces)
        {
            if (namespaces == null) throw new ArgumentNullException(nameof(namespaces));
#if NET40
            return new MultiConfig(await TaskEx.WhenAll(namespaces.Reverse().Distinct().Select(GetConfig)).ConfigureAwait(false));
#else
            return new MultiConfig(await Task.WhenAll(namespaces.Reverse().Distinct().Select(GetConfig)).ConfigureAwait(false));
#endif
        }
    }
}

