using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Com.Ctrip.Framework.Apollo.Core.Ioc.LightInject;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc.Extensions.Annotation
{
    /// <summary>
    /// A <see cref="ConstructorDependencySelector"/> that looks for the <see cref="InjectAttribute"/> 
    /// to determine the name of service to be injected.
    /// </summary>
    internal class AnnotatedConstructorDependencySelector : ConstructorDependencySelector
    {
        /// <summary>
        /// Selects the constructor dependencies for the given <paramref name="constructor"/>.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructionInfo"/> for which to select the constructor dependencies.</param>
        /// <returns>A list of <see cref="ConstructorDependency"/> instances that represents the constructor
        /// dependencies for the given <paramref name="constructor"/>.</returns>
        public override IEnumerable<ConstructorDependency> Execute(ConstructorInfo constructor)
        {
            var constructorDependencies = base.Execute(constructor).ToArray();
            foreach (var constructorDependency in constructorDependencies)
            {
                var injectAttribute =
                    (InjectAttribute)
                    constructorDependency.Parameter.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();
                if (injectAttribute != null)
                {
                    constructorDependency.ServiceName = injectAttribute.ServiceName;
                }
            }

            return constructorDependencies;
        }
    }
}
