using System;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public interface ConfigFactory
    {
        /// <summary>
        /// Create the config instance for the namespace.
        /// </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the newly created config instance </returns>
        Config Create(string namespaceName);
    }
}

