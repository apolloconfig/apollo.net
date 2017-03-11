using System;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    public interface IProviderManager
    {
        string GetProperty(string name, string defaultValue);

        IProvider Provider(Type clazz);
    }
}
