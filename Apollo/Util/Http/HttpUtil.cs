using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.IO;
using Com.Ctrip.Framework.Apollo.Exceptions;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    [Named(ServiceType = typeof(HttpUtil))]
    public class HttpUtil
    {
        [Inject]
        private ConfigUtil m_configUtil;
        private string basicAuth;

        public HttpUtil()
        {
            basicAuth = "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("user:"));
        }

        public HttpResponse<T> DoGet<T>(HttpRequest httpRequest)
        {
            int statusCode;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(httpRequest.Url);
                req.Method = "GET";
                req.Headers["Authorization"] = basicAuth;

                int timeout = httpRequest.Timeout;
                if (timeout <= 0 && timeout != Timeout.Infinite)
                {
                    timeout = m_configUtil.Timeout;
                }

                int readTimeout = httpRequest.ReadTimeout;
                if (readTimeout <= 0 && readTimeout != Timeout.Infinite)
                {
                    readTimeout = m_configUtil.ReadTimeout;
                }

                req.Timeout = timeout;
                req.ReadWriteTimeout = readTimeout;

                using (HttpWebResponse res = (HttpWebResponse)req.BetterGetResponse())
                {
                    statusCode = (int)res.StatusCode;
                    if (statusCode == 200)
                    {
                        using (var stream = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                        {
                            T body = JSON.DeserializeObject<T>(stream.ReadToEnd());
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
