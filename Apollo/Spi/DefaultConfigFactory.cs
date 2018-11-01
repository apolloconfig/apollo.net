using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo.Internals;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public class DefaultConfigFactory : IConfigFactory
    {
        private readonly ConfigRepositoryFactory _repositoryFactory;

        public DefaultConfigFactory(ConfigRepositoryFactory repositoryFactory) => _repositoryFactory = repositoryFactory;

        public async Task<IConfig> Create(string namespaceName)
        {
            var configRepository = _repositoryFactory.GetConfigRepository(namespaceName);

            var config = new DefaultConfig(namespaceName, configRepository);

            await config.Initialize().ConfigureAwait(false);

            return config;
        }
    }
}
