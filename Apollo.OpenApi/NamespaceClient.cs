using System;
using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public class NamespaceClient : OpenApiClient, INamespaceClient
    {
        public NamespaceClient(Func<HttpClient> httpClientFactory, string appId, string env, string cluster, string @namespace)
            : base(httpClientFactory)
        {
            AppId = appId;
            Env = env;
            Cluster = cluster;
            Namespace = @namespace;
        }

        public string AppId { get; }
        public string Env { get; }
        public string Cluster { get; }
        public string Namespace { get; }
    }
}
