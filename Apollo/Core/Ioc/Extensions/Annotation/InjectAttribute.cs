using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    /// <summary>
    /// Indicates a required property dependency or a named constructor dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> class.
        /// </summary>
        public InjectAttribute()
            : this(string.Empty)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> class.
        /// </summary>
        /// <param name="serviceName">The name of the service to be injected.</param>
        public InjectAttribute(string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the name of the service to be injected.
        /// </summary>
        public string ServiceName { get; private set; }
    }
}
