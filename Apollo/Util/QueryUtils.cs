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
            if (source == null || source.Count == 0)
            {
                return "";
            }
            var sb = new StringBuilder(source.Count * 32);

            foreach (var kv in source)
            {
                sb.Append($"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}&");
            }

            return sb.ToString(1, sb.Length - 1);
        }
    }
}
