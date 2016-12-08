using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    /// <summary>
    /// Providers a default container.
    /// </summary>
    public class VenusContainerLoader
    {
        private static readonly IVenusContainer container = new VenusContainer();

        private VenusContainerLoader()
        { }

        /// <summary>
        /// Gets the default container instance.
        /// </summary>
        public static IVenusContainer Container
        {
            get { return container; }
        }
    }
}
