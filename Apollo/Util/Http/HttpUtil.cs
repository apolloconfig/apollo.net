using Com.Ctrip.Framework.Apollo.Exceptions;
using System;
using System.Net;
using System.Net.Http;
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

        public Task<HttpResponse<T>> DoGetAsync<T>(Uri url) => DoGetAsync<T>(url, _options.Timeout);

        public async Task<HttpResponse<T>> DoGetAsync<T>(Uri url, int timeout)
        {
            Exception e;
            try
            {
#if NET40
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(timeout);
#else
                using var cts = new CancellationTokenSource(timeout);
#endif
                var httpClient = new HttpClient(_httpMessageHandler, false) { Timeout = TimeSpan.FromMilliseconds(timeout > 0 ? timeout : _options.Timeout) };

                if (!string.IsNullOrWhiteSpace(_options.Secret))
                    foreach (var header in Signature.BuildHttpHeaders(url, _options.AppId, _options.Secret!))
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

                using var response = await Timeout(httpClient.GetAsync(url, cts.Token), timeout, cts).ConfigureAwait(false);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
#if NET40
                        return new HttpResponse<T>(response.StatusCode, await response.Content.ReadAsAsync<T>().ConfigureAwait(false));
#else
                        return new HttpResponse<T>(response.StatusCode, await response.Content.ReadAsAsync<T>(cts.Token).ConfigureAwait(false));
#endif
                    case HttpStatusCode.NotModified:
                        return new HttpResponse<T>(response.StatusCode);
                }

                e = new ApolloConfigStatusCodeException(response.StatusCode, $"Get operation failed for {url}");
            }
            catch (Exception ex)
            {
                e = new ApolloConfigException("Could not complete get operation", ex);
            }

            throw e;
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
