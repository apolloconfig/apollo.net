using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Com.Ctrip.Framework.Apollo.Core.Ioc.LightInject;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc.Extensions.Annotation
{
    /// <summary>
    /// Selects the constructor from a given type that is annotated with <see cref="InjectConstructorAttribute"/>, or it will choose the constructor with most resolvable parameters.
    /// </summary>
    internal class AnnotatedConstructorSelector : IConstructorSelector
    {
        private readonly Func<Type, string, bool> canGetInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="MostResolvableConstructorSelector"/> class.
        /// </summary>
        /// <param name="canGetInstance">A function delegate that determines if a service type can be resolved.</param>
        public AnnotatedConstructorSelector(Func<Type, string, bool> canGetInstance)
        {
            this.canGetInstance = canGetInstance;
        }

        /// <summary>
        /// Selects the constructor to be used when creating a new instance of the <paramref name="implementingType"/>.
        /// </summary>
        /// <param name="implementingType">The <see cref="Type"/> for which to return a <see cref="ConstructionInfo"/>.</param>
        /// <returns>A <see cref="ConstructionInfo"/> instance that represents the constructor to be used
        /// when creating a new instance of the <paramref name="implementingType"/>.</returns>
        public ConstructorInfo Execute(Type implementingType)
        {
            ConstructorInfo[] constructorCandidates = implementingType.GetConstructors();
            if (constructorCandidates.Length == 0)
            {
                throw new InvalidOperationException("Missing public constructor for Type: " + implementingType.FullName);
            }

            if (constructorCandidates.Length == 1)
            {
                return constructorCandidates[0];
            }

            ConstructorInfo constructor = null;
            foreach (var constructorCandidate in constructorCandidates.OrderByDescending(c => c.GetParameters().Count()))
            {
                ParameterInfo[] parameters = constructorCandidate.GetParameters();
                if (CanCreateParameterDependencies(parameters))
                {
                    if (constructorCandidate.IsDefined(typeof(InjectConstructorAttribute), true))
                    {
                        constructor = constructorCandidate;
                        break;
                    }
                    else
                    {
                        if (constructor == null)
                            constructor = constructorCandidate;
                    }
                }
            }

            if (constructor != null)
                return constructor;

            throw new InvalidOperationException("No resolvable constructor found for Type: " + implementingType.FullName);
        }

        /// <summary>
        /// Gets the service name based on the given <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterInfo"/> for which to get the service name.</param>
        /// <returns>The name of the service for the given <paramref name="parameter"/>.</returns>
        private string GetServiceName(ParameterInfo parameter)
        {
            var injectAttribute =
                      (InjectAttribute)
                      parameter.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault();

            return injectAttribute != null ? injectAttribute.ServiceName : parameter.Name;
        }

        private bool CanCreateParameterDependencies(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.All(CanCreateParameterDependency);
        }

        private bool CanCreateParameterDependency(ParameterInfo parameterInfo)
        {
            return canGetInstance(parameterInfo.ParameterType, string.Empty) || canGetInstance(parameterInfo.ParameterType, GetServiceName(parameterInfo));
        }
    }
}
