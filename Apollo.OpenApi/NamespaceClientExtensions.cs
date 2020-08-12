using Com.Ctrip.Framework.Apollo.OpenApi.Model;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
#if NET40
using WebUtility = System.Web.HttpUtility;
#endif

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public static class NamespaceClientExtensions
    {
        /// <summary>获取信息</summary>
        public static Task<Namespace?> GetNamespaceInfo(this INamespaceClient client,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return client.Get<Namespace>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}", cancellationToken);
        }

        /// <summary>获取当前编辑人</summary>
        public static Task<NamespaceLock?> GetNamespaceLock(this INamespaceClient client,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return client.Get<NamespaceLock>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/lock", cancellationToken);
        }

        /// <summary>获取配置</summary>
        public static Task<Item?> GetItem(this INamespaceClient client,
             string key, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (key == null) throw new ArgumentNullException(nameof(key));

            return client.Get<Item>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/items/{key}", cancellationToken);
        }

        /// <summary>新增配置</summary>
        public static Task<Item> CreateItem(this INamespaceClient client,
             Item item, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrEmpty(item.Key)) throw new ArgumentNullException($"{nameof(item)}.{nameof(item.Key)}");
            if (string.IsNullOrEmpty(item.DataChangeCreatedBy)) throw new ArgumentNullException($"{nameof(item)}.{nameof(item.DataChangeCreatedBy)}");

            return client.Post<Item>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/items", item, cancellationToken);
        }

        /// <summary>修改配置</summary>
        public static Task UpdateItem(this INamespaceClient client,
             Item item, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrEmpty(item.Key)) throw new ArgumentNullException($"{nameof(item)}.{nameof(item.Key)}");
            if (string.IsNullOrEmpty(item.DataChangeLastModifiedBy)) throw new ArgumentNullException($"{nameof(item)}.{nameof(item.DataChangeLastModifiedBy)}");

            return client.Put<Item>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/items/{item.Key}", item, cancellationToken);
        }

        /// <summary>创建或修改配置</summary>
        public static Task<Item> CreateOrUpdateItem(this INamespaceClient client,
             Item item, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrEmpty(item.Key)) throw new ArgumentNullException($"{nameof(item)}.{nameof(item.Key)}");
            if (string.IsNullOrEmpty(item.DataChangeCreatedBy)) throw new ArgumentNullException($"{nameof(item)}.{nameof(item.DataChangeCreatedBy)}");

            if (string.IsNullOrEmpty(item.DataChangeLastModifiedBy))
                item.DataChangeLastModifiedBy = item.DataChangeCreatedBy;

            return client.Put<Item>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/items/{item.Key}?createIfNotExists=true", item, cancellationToken);
        }

        /// <summary>删除配置</summary>
        /// <returns>存在时删除后返回true，或者返回false</returns>
        public static Task<bool> RemoveItem(this INamespaceClient client, string key,
             string @operator, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));

            return client.Delete($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/items/{key}?operator={WebUtility.UrlEncode(@operator)}", cancellationToken);
        }

        /// <summary>发布配置</summary>
        public static Task<Release> Publish(this INamespaceClient client,
             NamespaceRelease release, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (release == null) throw new ArgumentNullException(nameof(release));
            if (string.IsNullOrEmpty(release.ReleaseTitle)) throw new ArgumentNullException($"{nameof(release)}.{nameof(release.ReleaseTitle)}");
            if (string.IsNullOrEmpty(release.ReleasedBy)) throw new ArgumentNullException($"{nameof(release)}.{nameof(release.ReleasedBy)}");

            return client.Post<Release>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/releases", release, cancellationToken);
        }

        /// <summary>获取当前生效的已发布配置接口</summary>
        public static Task<Release?> GetLatestActiveRelease(this INamespaceClient client,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return client.Get<Release>($"envs/{client.Env}/apps/{client.AppId}/clusters/{client.Cluster}/namespaces/{client.Namespace}/releases/latest", cancellationToken);
        }

        /// <summary>回滚已发布配置接口</summary>
        public static Task<bool> Rollback(this INamespaceClient client, string @operator, int releaseId,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            return client.Put($"envs/{client.Env}/releases/{releaseId}/rollback?operator={WebUtility.UrlEncode(@operator)}", cancellationToken);
        }
    }
}
