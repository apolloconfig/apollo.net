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
        /// <param name="namespace"> the namespace of this change </param>
        /// <param name="changes"> the actual changes </param>
        public ConfigChangeEventArgs(string @namespace, IReadOnlyDictionary<string, ConfigChange> changes)
        {
            Namespace = @namespace;
            Changes = changes;
        }

        /// <summary>
        /// Get the keys changed. </summary>
        /// <returns> the list of the keys </returns>
        public IReadOnlyDictionary<string, ConfigChange> Changes { get; }

        /// <summary>
        /// Get the namespace of this change event. </summary>
        /// <returns> the namespace </returns>
        public string Namespace { get; }
    }
}
