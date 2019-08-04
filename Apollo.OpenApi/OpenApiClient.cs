using System;
using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    public class OpenApiClient : IOpenApiClient
    {
        private readonly Func<HttpClient> _httpClientFactory;

        public OpenApiClient(Func<HttpClient> httpClientFactory) => _httpClientFactory = httpClientFactory;

        public HttpClient CreateHttpClient() => _httpClientFactory();
    }
}
