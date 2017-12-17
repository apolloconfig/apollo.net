using System;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public interface IConfigRegistry
    {
        /// <summary>
        /// Register the config factory for the namespace specified.
        /// </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <param name="factory">   the factory for this namespace </param>
        void Register(string namespaceName, IConfigFactory factory);

        /// <summary>
        /// Get the registered config factory for the namespace.
        /// </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the factory registered for this namespace </returns>
        IConfigFactory GetFactory(string namespaceName);
    }
}

