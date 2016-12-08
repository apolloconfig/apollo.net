using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    /// <summary>
    /// Indicates a implementing type that supports auto registration during assembly scan.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class NamedAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedAttribute"/> class.
        /// </summary>
        public NamedAttribute()
        { }

        /// <summary>
        /// The service type to register.
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// The name of the service.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// The lifetime type of the registered service.
        /// </summary>
        public Type LifetimeType { get; set; }
    }
}
