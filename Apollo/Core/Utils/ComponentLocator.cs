using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using System.Collections.Concurrent;

namespace Com.Ctrip.Framework.Apollo.Core.Utils
{
    public class ComponentLocator
    {
        private static ConcurrentDictionary<Type, object> componentsCache = new ConcurrentDictionary<Type, object>();

        private static ConcurrentDictionary<Pair<Type, string>, object> componentsWithRoleHintCache = new ConcurrentDictionary<Pair<Type, string>, object>();


        static IVenusContainer container;
        static object syncRoot = new object();

        public static void DefineComponents(Action<IVenusContainer> define)
        {
            if (container == null)
            {
                lock (syncRoot)
                {
                    if (container == null)
                    {
                        container = new VenusContainer();
                        define(container);
                    }
                }
            }
        }

        public static T Lookup<T>()
        {
            object component = null;
            componentsCache.TryGetValue(typeof(T), out component);

            if (component == null)
            {
                component = container.Lookup<T>();
                componentsCache.TryAdd(typeof(T), component);
            }

            return (T)component;
        }

        public static T Lookup<T>(string roleHint)
        {
            Pair<Type, string> key = new Pair<Type, string>(typeof(T), roleHint);

            object component = null;
            componentsWithRoleHintCache.TryGetValue(key, out component);

            if (component == null)
            {
                component = container.Lookup<T>(roleHint);
                componentsWithRoleHintCache.TryAdd(key, component);
            }

            return (T)component;
        }

        public static IList<T> LookupList<T>()
        {
            return new List<T>(container.LookupList<T>());
        }

        public static IDictionary<string, T> LookupMap<T>()
        {
            return container.LookupMap<T>();
        }
    }
}
