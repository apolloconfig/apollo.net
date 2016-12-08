using System;
using Com.Ctrip.Framework.Apollo.Core.Utils;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public interface ConfigRepository
    {
        /// <summary>
        /// Get the config from this repository. </summary>
        /// <returns> config </returns>
        Properties GetConfig();

        /// <summary>
        /// Set the upstream repo for this repository. </summary>
        /// <param name="upstreamConfigRepository"> the upstream repo </param>
        void SetUpstreamRepository(ConfigRepository upstreamConfigRepository);

        /// <summary>
        /// Add change listener. </summary>
        /// <param name="listener"> the listener to observe the changes </param>
        void AddChangeListener(RepositoryChangeListener listener);

        /// <summary>
        /// Remove change listener. </summary>
        /// <param name="listener"> the listener to remove </param>
        void RemoveChangeListener(RepositoryChangeListener listener);
    }
}

