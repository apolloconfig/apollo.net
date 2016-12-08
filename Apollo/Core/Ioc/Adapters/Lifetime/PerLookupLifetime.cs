using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    /// <summary>
    /// Ensures that a new instance is created for each lookup.
    /// </summary>
    public class PerLookupLifetime : ILifetime
    {
        /// <summary>
        /// Returns a service instance according to the specific lifetime characteristics.
        /// </summary>
        /// <param name="createInstance">The function delegate used to create a new service instance.</param>
        /// <returns>The requested services instance.</returns>
        public object GetInstance(Func<object> createInstance)
        {
            return createInstance();
        }
    }
}
