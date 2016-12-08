using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc.Utility
{
    internal class NamespaceList
    {
        private Dictionary<string, NamespaceList> index = new Dictionary<string, NamespaceList>();
        private int level;

        private NamespaceList(int level)
        {
            this.level = level;
        }

        public static NamespaceList Create()
        {
            return new NamespaceList(1);
        }

        public int Count
        {
            get
            {
                return index.Count;
            }
        }

        public void Add(string[] ns)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");

            if (ns.Length >= level)
            {
                var key = ns[level - 1].Trim();
                if (!index.ContainsKey(key))
                {
                    if (ns.Length == level)
                    {
                        index.Add(key, null);
                    }
                    else
                    {
                        var list = new NamespaceList(level + 1);
                        list.Add(ns);
                        index.Add(key, list);
                    }
                }
                else
                {
                    if (ns.Length > level)
                    {
                        var list = index[key];
                        if (list != null)
                            list.Add(ns);
                    }
                }
            }
        }

        public bool Include(string[] ns)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");

            if (ns.Length >= level)
            {
                var key = ns[level - 1].Trim();
                if (index.ContainsKey(key))
                {
                    var list = index[key];
                    if (list == null)
                        return true;

                    return list.Include(ns);
                }
            }

            return false;
        }
    }
}
