using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    public class HttpUtil
    {
        private readonly IApolloOptions _options;
        private readonly string _basicAuth;

        public HttpUtil(IApolloOptions options)
        {
            _options = options;
            _basicAuth = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:"));
        }

        public HttpResponse<T> DoGet<T>(HttpRequest httpRequest)
        {
            int statusCode;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(httpRequest.Url);
                req.Method = "GET";
                req.Headers["Authorization"] = _basicAuth;

                var timeout = httpRequest.Timeout;
                if (timeout <= 0 && timeout != Timeout.Infinite)
                {
                    timeout = _options.Timeout;
                }

                var readTimeout = httpRequest.ReadTimeout;
                if (readTimeout <= 0 && readTimeout != Timeout.Infinite)
                {
                    readTimeout = _options.ReadTimeout;
                }

                req.Timeout = timeout;
                req.ReadWriteTimeout = readTimeout;

                using (var res = (HttpWebResponse)req.BetterGetResponse())
                {
                    statusCode = (int)res.StatusCode;
                    if (statusCode == 200)
                    {
                        using (var stream = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                        {
                            var body = JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
                            return new HttpResponse<T>(statusCode, body);
                        }
                    }

                    if (statusCode == 304)
                    {
                        return new HttpResponse<T>(statusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApolloConfigException("Could not complete get operation", ex);
            }

            throw new ApolloConfigStatusCodeException(statusCode, string.Format("Get operation failed for {0}", httpRequest.Url));
        }

        public async Task<HttpResponse<T>> DoGetAsync<T>(HttpRequest httpRequest)
        {
            int statusCode;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(httpRequest.Url);
                req.Method = "GET";
                req.Headers["Authorization"] = _basicAuth;

                var timeout = httpRequest.Timeout;
                if (timeout <= 0 && timeout != Timeout.Infinite)
                {
                    timeout = _options.Timeout;
                }

                var readTimeout = httpRequest.ReadTimeout;
                if (readTimeout <= 0 && readTimeout != Timeout.Infinite)
                {
                    readTimeout = _options.ReadTimeout;
                }

                req.Timeout = timeout;
                req.ReadWriteTimeout = readTimeout;

                using (var res = (HttpWebResponse)await req.BetterGetResponseAsync())
                {
                    statusCode = (int)res.StatusCode;
                    if (statusCode == 200)
                    {
                        using (var stream = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                        {
                            var body = JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
                            return new HttpResponse<T>(statusCode, body);
                        }
                    }

                    if (statusCode == 304)
                    {
                        return new HttpResponse<T>(statusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApolloConfigException("Could not complete get operation", ex);
            }

            throw new ApolloConfigStatusCodeException(statusCode, string.Format("Get operation failed for {0}", httpRequest.Url));
        }
    }
}
