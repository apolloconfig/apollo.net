using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.OpenApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
#if NET40
using WebUtility = System.Web.HttpUtility;
#endif

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public static class AppClusterClientExtensions
    {
        /// <summary>获取App的环境，集群信息</summary>
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

        /// <summary>获取App信息</summary>
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

        /// <summary>获取集群</summary>
        public static Task<Cluster?> GetCluster(this IAppClusterClient client, string env,
            string clusterName = ConfigConsts.ClusterNameDefault,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (env == null) throw new ArgumentNullException(nameof(env));

            return client.Get<Cluster>($"envs/{env}/apps/{client.AppId}/clusters/{clusterName}", cancellationToken);
        }

        /// <summary>创建集群</summary>
        public static Task CreateCluster(this IAppClusterClient client, string env,
            Cluster cluster,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (env == null) throw new ArgumentNullException(nameof(env));

            return client.Post<Cluster>($"envs/{env}/apps/{client.AppId}/clusters", cluster, cancellationToken);
        }

        /// <summary>获取App信息</summary>
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

        /// <summary>获取集群下所有Namespace信息</summary>
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

        /// <summary>创建Namespace，可以通过此接口创建Namespace，调用此接口需要授予第三方APP对目标APP的管理权限。</summary>
        public static Task<AppNamespace> CreateAppNamespace(this IAppClusterClient client,
             AppNamespace appNamespace, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (appNamespace == null) throw new ArgumentNullException(nameof(appNamespace));

            return client.Post<AppNamespace>($"apps/{client.AppId}/appnamespaces", appNamespace, cancellationToken);
        }
    }
}
