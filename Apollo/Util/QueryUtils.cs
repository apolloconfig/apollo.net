using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Util
{
    static class QueryUtils
    {
        public static string Build(IReadOnlyCollection<KeyValuePair<string, string>> source)
        {
            var sb = new StringBuilder(source.Count * 32);

            foreach (var kv in source)
            {
                sb.Append('&');
                sb.Append(WebUtility.UrlEncode(kv.Key));
                sb.Append('=');
                sb.Append(WebUtility.UrlEncode(kv.Value));
            }

            return sb.ToString(0, sb.Length - 1);
        }
    }
}
