using System;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public interface IConfigFactoryManager
    {
        /// <summary>
        /// Get the config factory for the namespace.
        /// </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the config factory for this namespace </returns>
        IConfigFactory GetFactory(string namespaceName);
    }
}

