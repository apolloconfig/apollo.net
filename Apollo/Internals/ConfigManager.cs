using System;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public interface ConfigManager
    {
        /// <summary>
        /// Get the config instance for the namespace specified. </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the config instance for the namespace </returns>
        Config GetConfig(string namespaceName);
    }
}

