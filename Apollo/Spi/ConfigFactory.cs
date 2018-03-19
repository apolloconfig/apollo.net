using System;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public interface IConfigFactory
    {
        /// <summary>
        /// Create the config instance for the namespace.
        /// </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the newly created config instance </returns>
        Task<IConfig> Create(string namespaceName);
    }
}

