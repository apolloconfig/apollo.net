using System;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public interface ConfigFactoryManager
    {
        /// <summary>
        /// Get the config factory for the namespace.
        /// </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the config factory for this namespace </returns>
        ConfigFactory GetFactory(string namespaceName);
    }
}

