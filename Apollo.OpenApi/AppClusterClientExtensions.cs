using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.OpenApi.Model;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public static class AppClusterClientExtensions
    {
        /// <summary>获取App的环境，集群信息</summary>
        public static Task<IReadOnlyList<EnvCluster>?> GetEnvClusterInfo([NotNull] this IAppClusterClient client,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return client.Get<IReadOnlyList<EnvCluster>>($"apps/{client.AppId}/envclusters", cancellationToken);
        }

        /// <summary>获取App信息</summary>
        public static async Task<AppInfo?> GetAppInfo([NotNull] this IAppClusterClient client, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            var list = await client.Get<IReadOnlyList<AppInfo>>("apps?appIds=" + WebUtility.UrlEncode(client.AppId), cancellationToken).ConfigureAwait(false);

            return list?.FirstOrDefault();
        }

        /// <summary>获取App信息</summary>
        public static Task<IReadOnlyList<AppInfo>?> GetAppsInfo([NotNull] this IAppClusterClient client,
            IReadOnlyCollection<string>? appIds = null, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            if (appIds == null || appIds.Count == 0)
                return client.Get<IReadOnlyList<AppInfo>>("apps", cancellationToken);

            return client.Get<IReadOnlyList<AppInfo>>($"apps?appIds={WebUtility.UrlEncode(string.Join(",", appIds))}", cancellationToken);
        }

        /// <summary>获取集群下所有Namespace信息</summary>
        public static Task<IReadOnlyList<Namespace>?> GetNamespaces([NotNull] this IAppClusterClient client, [NotNull] string env,
            string clusterName = ConfigConsts.ClusterNameDefault,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (env == null) throw new ArgumentNullException(nameof(env));

            return client.Get<IReadOnlyList<Namespace>>($"envs/{env}/apps/{client.AppId}/clusters/{clusterName}/namespaces", cancellationToken);
        }

        /// <summary>创建Namespace</summary>
        public static Task<AppNamespace> CreateAppNamespace([NotNull] this IAppClusterClient client,
            [NotNull] AppNamespace appNamespace, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (appNamespace == null) throw new ArgumentNullException(nameof(appNamespace));

            return client.Post<AppNamespace>($"apps/{client.AppId}/appnamespaces", appNamespace, cancellationToken);
        }
    }
}
