using System;
using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public class AppClusterClient : OpenApiClient, IAppClusterClient
    {
        public AppClusterClient(Func<HttpClient> httpClientFactory, string appId)
            : base(httpClientFactory) => AppId = appId;

        public string AppId { get; }
    }
}
