using System;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    internal interface IProviderManager
    {
        string GetProperty(string name, string defaultValue);

        IProvider Provider(Type clazz);
    }
}
