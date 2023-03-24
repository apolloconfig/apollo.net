using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.OpenApi.Model;
#if NET40
using WebUtility = System.Web.HttpUtility;
#endif

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public static class AppClusterClientExtensions
    {
        /// <summary>3.2.1 获取App的环境，集群信息</summary>
        /// <see href="https://www.apolloconfig.com/#/zh/usage/apollo-open-api-platform?id=_321-%e8%8e%b7%e5%8f%96app%e7%9a%84%e7%8e%af%e5%a2%83%ef%bc%8c%e9%9b%86%e7%be%a4%e4%bf%a1%e6%81%af" />
#if NET40
        public static Task<IList<EnvCluster>?> GetEnvClusterInfo(this IAppClusterClient client,
#else
        public static Task<IReadOnlyList<EnvCluster>?> GetEnvClusterInfo(this IAppClusterClient client,
#endif
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
#if NET40
            return client.Get<IList<EnvCluster>>($"apps/{client.AppId}/envclusters", cancellationToken);
#else
            return client.Get<IReadOnlyList<EnvCluster>>($"apps/{client.AppId}/envclusters", cancellationToken);
#endif
        }

        /// <summary>3.2.2 获取App信息</summary>
        /// <see href="https://www.apolloconfig.com/#/zh/usage/apollo-open-api-platform?id=_322-%e8%8e%b7%e5%8f%96app%e4%bf%a1%e6%81%af" />
        public static async Task<AppInfo?> GetAppInfo(this IAppClusterClient client, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
#if NET40
            var list = await client.Get<IList<AppInfo>>("apps?appIds=" + WebUtility.UrlEncode(client.AppId), cancellationToken).ConfigureAwait(false);
#else
            var list = await client.Get<IReadOnlyList<AppInfo>>("apps?appIds=" + WebUtility.UrlEncode(client.AppId), cancellationToken).ConfigureAwait(false);
#endif
            return list?.FirstOrDefault();
        }

        /// <summary>3.2.2 获取App信息</summary>
        /// <see href="https://www.apolloconfig.com/#/zh/usage/apollo-open-api-platform?id=_322-%e8%8e%b7%e5%8f%96app%e4%bf%a1%e6%81%af" />
#if NET40
        public static Task<IList<AppInfo>?> GetAppsInfo(this IAppClusterClient client,
            ICollection<string>? appIds = null, CancellationToken cancellationToken = default)
#else
        public static Task<IReadOnlyList<AppInfo>?> GetAppsInfo(this IAppClusterClient client,
            IReadOnlyCollection<string>? appIds = null, CancellationToken cancellationToken = default)
#endif
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            if (appIds == null || appIds.Count == 0)
#if NET40
                return client.Get<IList<AppInfo>>("apps", cancellationToken);

            return client.Get<IList<AppInfo>>($"apps?appIds={WebUtility.UrlEncode(string.Join(",", appIds))}", cancellationToken);
#else
                return client.Get<IReadOnlyList<AppInfo>>("apps", cancellationToken);

            return client.Get<IReadOnlyList<AppInfo>>($"apps?appIds={WebUtility.UrlEncode(string.Join(",", appIds))}", cancellationToken);
#endif
        }

        /// <summary>3.2.3 获取集群接口</summary>
        /// <see href="https://www.apolloconfig.com/#/zh/usage/apollo-open-api-platform?id=_323-%e8%8e%b7%e5%8f%96%e9%9b%86%e7%be%a4%e6%8e%a5%e5%8f%a3" />
        public static Task<Cluster?> GetCluster(this IAppClusterClient client, string env,
            string clusterName = ConfigConsts.ClusterNameDefault,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (env == null) throw new ArgumentNullException(nameof(env));

            return client.Get<Cluster>($"envs/{env}/apps/{client.AppId}/clusters/{clusterName}", cancellationToken);
        }

        /// <summary>3.2.4 创建集群接口</summary>
        /// <see href="https://www.apolloconfig.com/#/zh/usage/apollo-open-api-platform?id=_324-%e5%88%9b%e5%bb%ba%e9%9b%86%e7%be%a4%e6%8e%a5%e5%8f%a3" />
        public static Task CreateCluster(this IAppClusterClient client, string env,
            Cluster cluster,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (env == null) throw new ArgumentNullException(nameof(env));

            return client.Post<Cluster>($"envs/{env}/apps/{client.AppId}/clusters", cluster, cancellationToken);
        }

        /// <summary>3.2.5 获取集群下所有Namespace信息接口</summary>
        /// <see href="https://www.apolloconfig.com/#/zh/usage/apollo-open-api-platform?id=_325-%e8%8e%b7%e5%8f%96%e9%9b%86%e7%be%a4%e4%b8%8b%e6%89%80%e6%9c%89namespace%e4%bf%a1%e6%81%af%e6%8e%a5%e5%8f%a3" />
#if NET40
        public static Task<IList<Namespace>?> GetNamespaces(this IAppClusterClient client, string env,
#else
        public static Task<IReadOnlyList<Namespace>?> GetNamespaces(this IAppClusterClient client, string env,
#endif
            string clusterName = ConfigConsts.ClusterNameDefault,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (env == null) throw new ArgumentNullException(nameof(env));
#if NET40
            return client.Get<IList<Namespace>>($"envs/{env}/apps/{client.AppId}/clusters/{clusterName}/namespaces", cancellationToken);
#else
            return client.Get<IReadOnlyList<Namespace>>($"envs/{env}/apps/{client.AppId}/clusters/{clusterName}/namespaces", cancellationToken);
#endif
        }

        /// <summary>3.2.7 创建Namespace。可以通过此接口创建Namespace，调用此接口需要授予第三方APP对目标APP的管理权限。</summary>
        /// <see href="https://www.apolloconfig.com/#/zh/usage/apollo-open-api-platform?id=_327-%e5%88%9b%e5%bb%banamespace" />
        public static Task<AppNamespace> CreateAppNamespace(this IAppClusterClient client,
            AppNamespace appNamespace, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (appNamespace == null) throw new ArgumentNullException(nameof(appNamespace));

            return client.Post<AppNamespace>($"apps/{client.AppId}/appnamespaces", appNamespace, cancellationToken);
        }
    }
}
