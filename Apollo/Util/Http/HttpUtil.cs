using Com.Ctrip.Framework.Apollo.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpUtil : IDisposable
    {
        private readonly ConcurrentDictionary<int, HttpClient> _httpClient = new ConcurrentDictionary<int, HttpClient>();
        private readonly IApolloOptions _options;

        public HttpUtil(IApolloOptions options) => _options = options;

        public async Task<HttpResponse<T>> DoGetAsync<T>(string url, int timeout = 0)
        {
            HttpResponseMessage response = null;
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

                if (!string.IsNullOrEmpty(_options.Authorization))
                    httpRequest.Headers.TryAddWithoutValidation(nameof(httpRequest.Headers.Authorization), _options.Authorization);

                response = await _httpClient.GetOrAdd(timeout > 0 ? timeout : _options.Timeout, t => new HttpClient { Timeout = TimeSpan.FromMilliseconds(t) }).SendAsync(httpRequest);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var s = await response.Content.ReadAsStreamAsync())
                    using (var sr = new StreamReader(s, Encoding.UTF8))
                    using (var jtr = new JsonTextReader(sr))
                        return new HttpResponse<T>(response.StatusCode, JsonSerializer.Create().Deserialize<T>(jtr));
                }

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    return new HttpResponse<T>(response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new ApolloConfigException("Could not complete get operation", ex);
            }
            finally
            {
                response?.Dispose();
            }

            throw new ApolloConfigStatusCodeException(response.StatusCode, string.Format("Get operation failed for {0}", url));
        }

        public void Dispose()
        {
            if (_httpClient == null)
                return;

            foreach (var httpClient in _httpClient.Values)
            {
                try
                {
                    httpClient.Dispose();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
