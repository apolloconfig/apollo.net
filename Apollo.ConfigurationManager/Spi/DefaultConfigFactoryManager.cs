using System.Collections.Concurrent;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public class DefaultConfigFactoryManager : IConfigFactoryManager
    {
        private readonly IConfigRegistry _registry;

        private readonly ConcurrentDictionary<string, IConfigFactory> _factories = new ConcurrentDictionary<string, IConfigFactory>();

        public DefaultConfigFactoryManager(IConfigRegistry registry) => _registry = registry;

        public IConfigFactory GetFactory(string namespaceName) =>
            _registry.GetFactory(namespaceName) ??
            _factories.GetOrAdd(namespaceName, _ => new DefaultConfigFactory());
    }
}

