using System;

namespace Com.Ctrip.Framework.Foundation.Spi.Provider
{
    internal interface IProvider
    {
        Type Type { get; }
        string Property(string name, string defaultValue);
        void Initialize();
    }
}
