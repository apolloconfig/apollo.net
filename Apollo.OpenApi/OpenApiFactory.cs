using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public class OpenApiFactory : IOpenApiFactory
    {
        private static readonly Lazy<HttpMessageHandler> Handler = new Lazy<HttpMessageHandler>(() => new HttpClientHandler());
        private readonly Func<HttpClient> _httpClientFactory;
        private readonly OpenApiOptions _options;
        private readonly Uri _baseUri;

        public OpenApiFactory(OpenApiOptions options) : this(options, () => Handler.Value, false) { }

        public OpenApiFactory(OpenApiOptions options, Func<HttpMessageHandler> httpMessageHandlerFactory)
            : this(options, httpMessageHandlerFactory, true) { }

        public OpenApiFactory(OpenApiOptions options, Func<HttpMessageHandler> httpMessageHandlerFactory, bool disposeHandler)
        {
            if (options.PortalUrl == null) throw new ArgumentNullException($"{nameof(options)}.{nameof(options.PortalUrl)}");
            if (string.IsNullOrEmpty(options.Token)) throw new ArgumentNullException($"{nameof(options)}.{nameof(options.Token)}");
            _options = options;

            _baseUri = new UriBuilder(options.PortalUrl) { Path = "/openapi/v1/" }.Uri;

            _httpClientFactory = CreateHttpClient(httpMessageHandlerFactory, disposeHandler);
        }

        private Func<HttpClient> CreateHttpClient(Func<HttpMessageHandler> httpMessageHandlerFactory, bool disposeHandler) =>
            () => new HttpClient(httpMessageHandlerFactory(), disposeHandler)
            {
                BaseAddress = _baseUri,
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue(_options.Token) },
                Timeout = TimeSpan.FromMilliseconds(_options.Timeout)
            };

        public IAppClusterClient CreateAppClusterClient(string appId) =>
            new AppClusterClient(_httpClientFactory, appId);

        public INamespaceClient CreateNamespaceClient(string appId, string env, string cluster, string @namespace) =>
            new NamespaceClient(_httpClientFactory, appId, env, cluster, @namespace);
    }
}
