
using System;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    /// <summary>
    /// Represents a class that manages the lifetime of a service instance.
    /// </summary>
    public interface ILifetime
    {
        /// <summary>
        /// Returns a service instance according to the specific lifetime characteristics.
        /// </summary>
        /// <param name="createInstance">The function delegate used to create a new service instance.</param>
        /// <returns>The requested services instance.</returns>
        object GetInstance(Func<object> createInstance);
    }
}
