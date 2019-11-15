using Com.Ctrip.Framework.Apollo.Spi;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class DefaultConfigManager : IConfigManager
    {
        private readonly Dictionary<string, IConfig> _configs = new Dictionary<string, IConfig>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly DefaultConfigFactory _configFactory;

        public IConfigRegistry Registry { get; }

        public DefaultConfigManager(IConfigRegistry registry, IConfigRepositoryFactory repositoryFactory)
        {
            _configFactory = new DefaultConfigFactory(repositoryFactory);
            Registry = registry;
        }

        private IConfigFactory GetFactory(string namespaceName) => Registry.GetFactory(namespaceName) ?? _configFactory;

        public async Task<IConfig> GetConfig(string namespaceName)
        {
            if (_configs.TryGetValue(namespaceName, out var config)) return config;
#if NET40
            _semaphore.Wait();
#else
            await _semaphore.WaitAsync().ConfigureAwait(false);
#endif
            try
            {
                if (!_configs.TryGetValue(namespaceName, out config))
                    _configs[namespaceName] = config = await GetFactory(namespaceName).Create(namespaceName).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }

            return config;
        }
    }
}

