using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Util.Http
{
    internal class Signature
    {
        /// <summary>Authorization=Apollo {appId}:{sign}</summary>
        private const string AuthorizationFormat = "Apollo {0}:{1}";
        private const string Delimiter = "\n";
        private const string HttpHeaderAuthorization = "Authorization";
        private const string HttpHeaderTimestamp = "Timestamp";

        public static long GetTimeStamp()
        {
#if NETFRAMEWORK
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
#else
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
#endif
        }

        public static IDictionary<string, string> BuildHttpHeaders(Uri url, string appId, string secret)
        {
            var timestamp = GetTimeStamp().ToString();

            return new Dictionary<string, string>
            {
                {HttpHeaderAuthorization, string.Format(AuthorizationFormat, appId, SignString(timestamp + Delimiter + url.PathAndQuery, secret))},
                {HttpHeaderTimestamp, timestamp}
            };
        }

        public static string SignString(string data, string secret)
        {
            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(secret));

            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }
    }
}
