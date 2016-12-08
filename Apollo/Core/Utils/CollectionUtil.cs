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

        public static V TryGet<K, V>(IDictionary<K, V> d, K key)
        {
            V result = default(V);

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
            Random rnd = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

