using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    /// <summary>
    /// Ensures that only one instance of a given service can exist within the container. 
    /// </summary>
    public class PerContainerLifetime : ILifetime, IDisposable
    {
        private Com.Ctrip.Framework.Apollo.Core.Ioc.LightInject.PerContainerLifetime lifetime = new Com.Ctrip.Framework.Apollo.Core.Ioc.LightInject.PerContainerLifetime();

        /// <summary>
        /// Returns a service instance according to the specific lifetime characteristics.
        /// </summary>
        /// <param name="createInstance">The function delegate used to create a new service instance.</param>
        /// <returns>The requested services instance.</returns>
        public object GetInstance(Func<object> createInstance)
        {
            return lifetime.GetInstance(createInstance, null);
        }

        /// <summary>
        /// Disposes the service instances managed by this <see cref="PerContainerLifetime"/> instance.
        /// </summary>
        public void Dispose()
        {
            lifetime.Dispose();
        }
    }
}
