using Com.Ctrip.Framework.Apollo.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpUtil : IDisposable
    {
        private readonly HttpMessageHandler _httpMessageHandler;
        private readonly IApolloOptions _options;

        public HttpUtil(IApolloOptions options)
        {
            _options = options;

            _httpMessageHandler = _options.HttpMessageHandlerFactory == null ? new HttpClientHandler() : _options.HttpMessageHandlerFactory();
        }

        public Task<HttpResponse<T>> DoGetAsync<T>(string url) => DoGetAsync<T>(url, _options.Timeout);

        public async Task<HttpResponse<T>> DoGetAsync<T>(string url, int timeout)
        {
            HttpResponseMessage? response = null;
            try
            {
#if NET40
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(timeout);
#else
                using var cts = new CancellationTokenSource(timeout);
#endif
                var httpClient = new HttpClient(_httpMessageHandler, false) { Timeout = TimeSpan.FromMilliseconds(timeout > 0 ? timeout : _options.Timeout) };

                response = await Timeout(httpClient.GetAsync(url, cts.Token), timeout, cts).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    using var sr = new StreamReader(s, Encoding.UTF8);
                    using var jtr = new JsonTextReader(sr);
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

            throw new ApolloConfigStatusCodeException(response.StatusCode, $"Get operation failed for {url}");
        }

        public void Dispose() => _httpMessageHandler.Dispose();

        private static async Task<T> Timeout<T>(Task<T> task, int millisecondsDelay, CancellationTokenSource cts)
        {
#if NET40
            if (await TaskEx.WhenAny(task, TaskEx.Delay(millisecondsDelay, cts.Token)).ConfigureAwait(false) == task)
#else
            if (await Task.WhenAny(task, Task.Delay(millisecondsDelay, cts.Token)).ConfigureAwait(false) == task)
#endif
                return task.Result;

            cts.Cancel();

            throw new TimeoutException();
        }
    }
}
