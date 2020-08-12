using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.OpenApi
{
    internal static class OpenApiClientExtensions
    {
        private static readonly MediaTypeFormatter Json = new JsonMediaTypeFormatter
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        public static async Task<TResponse?> Get<TResponse>(this IOpenApiClient client, string url, CancellationToken cancellationToken) where TResponse : class
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            using var httpClient = client.CreateHttpClient();
            using var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound) return null;

            await AssertResponse(response).ConfigureAwait(false);
#if NET40
            return await response.Content.ReadAsAsync<TResponse>().ConfigureAwait(false);
#else
            return await response.Content.ReadAsAsync<TResponse>(cancellationToken).ConfigureAwait(false);
#endif
        }

        public static async Task<bool> Delete(this IOpenApiClient client, string url, CancellationToken cancellationToken)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            using var httpClient = client.CreateHttpClient();
            using var response = await httpClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound) return false;

            await AssertResponse(response).ConfigureAwait(false);

            return true;
        }

        public static async Task<TResponse> Post<TResponse>(this IOpenApiClient client, string url, object data, CancellationToken cancellationToken)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            using var httpClient = client.CreateHttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new ObjectContent<object>(data, Json)
            };
            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            await AssertResponse(response).ConfigureAwait(false);
#if NET40
            return await response.Content.ReadAsAsync<TResponse>().ConfigureAwait(false);
#else
            return await response.Content.ReadAsAsync<TResponse>(cancellationToken).ConfigureAwait(false);
#endif
        }

        public static async Task<bool> Put(this IOpenApiClient client, string url, CancellationToken cancellationToken)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            using var httpClient = client.CreateHttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Put, url);
            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound) return false;

            await AssertResponse(response).ConfigureAwait(false);

            return true;
        }

        public static async Task<TResponse> Put<TResponse>(this IOpenApiClient client, string url, object data, CancellationToken cancellationToken)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            using var httpClient = client.CreateHttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new ObjectContent<object>(data, Json)
            };
            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            await AssertResponse(response).ConfigureAwait(false);
#if NET40
            return await response.Content.ReadAsAsync<TResponse>().ConfigureAwait(false);
#else
            return await response.Content.ReadAsAsync<TResponse>(cancellationToken).ConfigureAwait(false);
#endif
        }

        private static async Task AssertResponse(HttpResponseMessage response)
        {
            if (response.StatusCode < HttpStatusCode.BadRequest) return;

            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Exception ex;
            try
            {
                var json = JsonConvert.DeserializeObject<JToken>(body);

                var exception = json.Value<string>("exception");
                var message = json.Value<string>("message");

                ex = new ApolloOpenApiException(response.StatusCode, string.IsNullOrEmpty(response.ReasonPhrase) ? exception : response.ReasonPhrase, message);
            }
            catch (Exception e)
            {
                ex = new ApolloOpenApiException(response.StatusCode, response.ReasonPhrase, body, e);
            }

            throw ex;
        }
    }
}
