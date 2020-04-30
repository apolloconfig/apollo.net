using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Signature
{
    /**
     * @author zero
     */
    public class Signature
    {
        /**
         * Authorization=Apollo {appId}:{sign}
         */
        private static readonly string AUTHORIZATION_FORMAT = "Apollo {0}:{1}";
        private static readonly string DELIMITER = "\n";

        public static string HTTP_HEADER_AUTHORIZATION = "Authorization";
        public static string HTTP_HEADER_TIMESTAMP = "Timestamp";

        public static string Sign(string timestamp, string pathWithQuery, string secret)
        {
            var stringToSign = timestamp + DELIMITER + pathWithQuery;

            return HmacSha1Utils.SignString(stringToSign, secret);
        }

        public static long GetTimeStamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        public static IDictionary<string, string> BuildHttpHeaders(string url, string appId, string secret)
        {
            var currentTimeMillis = GetTimeStamp();
            var timestamp = currentTimeMillis.ToString();

            var pathWithQuery = Url2PathWithQuery(url);

            var signature = Sign(timestamp, pathWithQuery, secret);

            var headers = new Dictionary<string, string>
            {
                { HTTP_HEADER_AUTHORIZATION,string.Format(AUTHORIZATION_FORMAT,appId, signature) },
                { HTTP_HEADER_TIMESTAMP,timestamp }
            };

            return headers;
        }

        private static string Url2PathWithQuery(string urlstring)
        {
            try
            {
                var url = new Uri(urlstring);
                var path = url.LocalPath.TrimEnd('?');
                var query = url.Query;

                var pathWithQuery = path;

                if (query != null && query.Length > 0)
                {
                    pathWithQuery += "?" + query.TrimStart('?');
                }

                return pathWithQuery;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Invalid url pattern: " + urlstring, e);
            }
        }
    }
}
