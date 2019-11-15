using System;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Model
{
    /// <summary>
    /// Config change event args
    /// </summary>
    public class ConfigChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor. </summary>
        /// <param name="config"> the namespace of this change </param>
        /// <param name="changes"> the actual changes </param>
#if NET40
        public ConfigChangeEventArgs(IConfig config, IDictionary<string, ConfigChange> changes)
#else
        public ConfigChangeEventArgs(IConfig config, IReadOnlyDictionary<string, ConfigChange> changes)
#endif
        {
            Config = config;
            Changes = changes;
        }

        /// <summary>
        /// Get the keys changed. </summary>
        /// <returns> the list of the keys </returns>
        public IEnumerable<string> ChangedKeys => Changes.Keys;

        /// <summary>
        /// Get a specific change instance for the key specified. </summary>
        /// <param name="key"> the changed key </param>
        /// <returns> the change instance </returns>
        public ConfigChange GetChange(string key)
        {
            Changes.TryGetValue(key, out var change);
            return change;
        }

        /// <summary>
        /// Check whether the specified key is changed </summary>
        /// <param name="key"> the key </param>
        /// <returns> true if the key is changed, false otherwise. </returns>
        public bool IsChanged(string key) => Changes.ContainsKey(key);

        /// <summary>
        /// Get the namespace of this change event. </summary>
        /// <returns> the namespace </returns>
        public IConfig Config { get; }
#if NET40
        public IDictionary<string, ConfigChange> Changes { get; }
#else
        public IReadOnlyDictionary<string, ConfigChange> Changes { get; }
#endif
    }
}
