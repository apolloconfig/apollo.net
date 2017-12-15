using Com.Ctrip.Framework.Foundation.Internals.Provider;
using Com.Ctrip.Framework.Foundation.Spi.Provider;
using System;

namespace Com.Ctrip.Framework.Foundation.Internals
{
    internal class NullProviderManager : IProviderManager
    {
        public static readonly NullProvider Provider = new NullProvider();

        public string GetProperty(string name, string defaultValue)
        {
            return defaultValue;
        }

        IProvider IProviderManager.Provider(Type clazz)
        {
            return Provider;
        }
    }
}
