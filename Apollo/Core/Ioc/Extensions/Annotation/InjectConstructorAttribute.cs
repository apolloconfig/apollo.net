using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc
{
    /// <summary>
    /// Indicates a required constructor injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public class InjectConstructorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectConstructorAttribute"/> class.
        /// </summary>
        public InjectConstructorAttribute()
        { }
    }
}
