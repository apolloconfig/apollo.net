using System;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{

    public static class CollectionUtil
    {
        public static bool IsNullOrEmpty<T>(List<T> list)
        {
            return list == null || list.Count == 0;
        }

        public static TV TryGet<TK, TV>(IDictionary<TK, TV> d, TK key)
        {
            var result = default(TV);

            if (d == null || key == null)
            {
                return result;
            }
            else
            {
                d.TryGetValue(key, out result);
                return result;
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var rnd = new Random();

            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rnd.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

