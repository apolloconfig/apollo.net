using Com.Ctrip.Framework.Apollo.Core;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public interface IOpenApiFactory
    {
        IAppClusterClient CreateAppClusterClient(string appId);

        INamespaceClient CreateNamespaceClient(string appId, string env,
            string cluster = ConfigConsts.ClusterNameDefault,
            string @namespace = ConfigConsts.NamespaceApplication);
    }
}
