using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Com.Ctrip.Framework.Apollo.Core.Ioc.LightInject;
using Com.Ctrip.Framework.Apollo.Core.Ioc.Adapters.Lifetime;

namespace Com.Ctrip.Framework.Apollo.Core.Ioc.Extensions.Annotation
{
    internal class AssemblyScanner : IAssemblyScanner
    {
        private ITypeExtractor namedTypeExtractor;

        public AssemblyScanner(ITypeExtractor namedTypeExtractor)
        {
            this.namedTypeExtractor = namedTypeExtractor;
        }

        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<LightInject.ILifetime> lifetimeFactory, Func<Type, Type, bool> shouldRegister)
        {
            var serviceTypes = namedTypeExtractor.Execute(assembly);
            foreach (var type in serviceTypes)
            {
                foreach (NamedAttribute attr in type.GetCustomAttributes(typeof(NamedAttribute), false))
                {
                    Type serviceType = attr.ServiceType;
                    if (serviceType == null)
                    {
                        var interfaces = type.GetInterfaces();
                        if (interfaces.Length != 1)
                        {
                            throw new ArgumentException("As service type not specified, a unique interface is required to be implemented or inherited by the type, or a base class is required to be inherited by the type, please supply the parameter value explicitly.", "ServiceType");
                        }

                        serviceType = interfaces[0];
                    }

                    LightInject.ILifetime lifetime;
                    if (attr.LifetimeType != null)
                    {
                        if (!typeof(ILifetime).IsAssignableFrom(attr.LifetimeType))
                        {
                            throw new ArgumentException("Type is not an implementation of the ILifetime interface.", "LifetimeType");
                        }

                        if (attr.LifetimeType == typeof(PerContainerLifetime))
                        {
                            lifetime = new LightInject.PerContainerLifetime();
                        }
                        else if (attr.LifetimeType == typeof(PerLookupLifetime))
                        {
                            lifetime = null;
                        }
                        else
                        {
                            lifetime = new AdapterLifetime((ILifetime)Activator.CreateInstance(attr.LifetimeType));
                        }
                    }
                    else
                    {
                        lifetime = new LightInject.PerContainerLifetime();
                    }

                    if (attr.ServiceName == null)
                    {
                        serviceRegistry.Register(serviceType, type, lifetime);
                    }
                    else
                    {
                        serviceRegistry.Register(serviceType, type, attr.ServiceName, lifetime);
                    }
                }
            }
        }

        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
        {
            return;
        }
    }
}
