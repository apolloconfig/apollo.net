using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo.Model;
using System;

namespace Com.Ctrip.Framework.Apollo.Model
{
    /// <summary>
    /// Config change event args
    /// </summary>
    public class ConfigChangeEventArgs : EventArgs
    {
        private readonly string m_namespace;
        private readonly IDictionary<string, ConfigChange> m_changes;

        /// <summary>
        /// Constructor. </summary>
        /// <param name="namespace"> the namespace of this change </param>
        /// <param name="changes"> the actual changes </param>
        public ConfigChangeEventArgs(string namespaceName, IDictionary<string, ConfigChange> changes)
        {
            m_namespace = namespaceName;
            m_changes = changes;
        }

        /// <summary>
        /// Get the keys changed. </summary>
        /// <returns> the list of the keys </returns>
        public ICollection<string> ChangedKeys
        {
            get
            {
                return m_changes.Keys;
            }
        }

        /// <summary>
        /// Get a specific change instance for the key specified. </summary>
        /// <param name="key"> the changed key </param>
        /// <returns> the change instance </returns>
        public ConfigChange GetChange(string key)
        {
            ConfigChange change;
            m_changes.TryGetValue(key, out change);
            return change;
        }

        /// <summary>
        /// Check whether the specified key is changed </summary>
        /// <param name="key"> the key </param>
        /// <returns> true if the key is changed, false otherwise. </returns>
        public bool IsChanged(string key)
        {
            return m_changes.ContainsKey(key);
        }

        /// <summary>
        /// Get the namespace of this change event. </summary>
        /// <returns> the namespace </returns>
        public string Namespace
        {
            get
            {
                return m_namespace;
            }
        }
    }
}
