using Com.Ctrip.Framework.Apollo.Spi;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class DefaultConfigManager : IConfigManager
    {
        private readonly IConfigFactoryManager _factoryManager;
        private readonly Dictionary<string, IConfig> _configs = new Dictionary<string, IConfig>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public DefaultConfigManager(IConfigFactoryManager factoryManager) => _factoryManager = factoryManager;

        public async Task<IConfig> GetConfig(string namespaceName)
        {
            if (!_configs.TryGetValue(namespaceName, out var config))
            {
                await _semaphore.WaitAsync();
                try
                {
                    if (!_configs.TryGetValue(namespaceName, out config))
                        _configs[namespaceName] = config = await _factoryManager.GetFactory(namespaceName).Create(namespaceName);
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return config;
        }
    }
}

