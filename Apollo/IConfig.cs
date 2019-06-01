using Com.Ctrip.Framework.Apollo.Model;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo
{
    /// <summary>
    /// Config change event fired when there is any config change for the namespace.
    /// </summary>
    /// <param name="sender"> the sender </param>
    /// <param name="args"> the changes </param>
    public delegate void ConfigChangeEvent(object sender, ConfigChangeEventArgs args);

    public interface IConfig
    {
        /// <summary>
        /// Return the property value with the given key, or
        /// {@code defaultValue} if the key doesn't exist. </summary>
        /// <param name="key"> the property name </param>
        /// <param name="value"> the default value when key is not found or any error occurred </param>
        /// <returns> the property value </returns>
        bool TryGetProperty(string key, out string value);

        /// <summary>
        /// Return a set of the property names
        /// </summary>
        /// <returns> the property names </returns>
        IEnumerable<string> GetPropertyNames();

        /// <summary>
        /// Config change event subscriber
        /// </summary>
        event ConfigChangeEvent ConfigChanged;
    }
}

