using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public class OpenApiFactory : IOpenApiFactory
    {
        private static readonly Lazy<HttpMessageHandler> Handler = new Lazy<HttpMessageHandler>(() => new HttpClientHandler());
        private readonly Func<HttpClient> _httpClientFactory;

        public OpenApiFactory(OpenApiOptions options) : this(options, () => Handler.Value, false) { }

        public OpenApiFactory(OpenApiOptions options, Func<HttpMessageHandler> httpMessageHandlerFactory)
            : this(options, httpMessageHandlerFactory, true) { }

        public OpenApiFactory(OpenApiOptions options, Func<HttpMessageHandler> httpMessageHandlerFactory, bool disposeHandler)
        {
            if (options.PortalUrl == null) throw new ArgumentNullException($"{nameof(options)}.{nameof(options.PortalUrl)}");
            if (string.IsNullOrEmpty(options.Token)) throw new ArgumentNullException($"{nameof(options)}.{nameof(options.Token)}");

            var baseUri = new UriBuilder(options.PortalUrl) { Path = "/openapi/v1/" }.Uri;
            var token = options.Token;

            _httpClientFactory = () => new HttpClient(httpMessageHandlerFactory(), disposeHandler)
            {
                BaseAddress = baseUri,
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue(token) },
                Timeout = TimeSpan.FromMilliseconds(options.Timeout)
            };
        }

        public IAppClusterClient CreateAppClusterClient(string appId) =>
            new AppClusterClient(_httpClientFactory, appId);

        public INamespaceClient CreateNamespaceClient(string appId, string env, string cluster, string @namespace) =>
            new NamespaceClient(_httpClientFactory, appId, env, cluster, @namespace);
    }
}
