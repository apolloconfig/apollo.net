using Com.Ctrip.Framework.Apollo.Spi;
using System.Collections.Concurrent;
using System.Threading;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class DefaultConfigManager : IConfigManager
    {
        private readonly IConfigFactoryManager _factoryManager;
        private readonly ConcurrentDictionary<string, IConfig> _configs = new ConcurrentDictionary<string, IConfig>();
        private  readonly  SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);

        public DefaultConfigManager(IConfigFactoryManager factoryManager) => _factoryManager = factoryManager;

        public IConfig GetConfig(string namespaceName) =>
            _configs.GetOrAdd(namespaceName, _ => _factoryManager.GetFactory(namespaceName).Create(_));
    }
}

