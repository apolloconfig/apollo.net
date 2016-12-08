using Com.Ctrip.Framework.Foundation.Internals.Provider;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Foundation.Internals
{
    class NullProviderManager : IProviderManager
    {
        public static readonly NullProvider provider = new NullProvider();

        public string GetProperty(string name, string defaultValue)
        {
            return defaultValue;
        }

        public IProvider Provider(Type clazz)
        {
            return provider;
        }
    }
}
