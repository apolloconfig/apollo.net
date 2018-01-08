using Com.Ctrip.Framework.Apollo.Internals;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public class DefaultConfigFactory : IConfigFactory
    {
        private readonly ConfigRepositoryFactory _repositoryFactory;

        public DefaultConfigFactory(ConfigRepositoryFactory repositoryFactory) => _repositoryFactory = repositoryFactory;

        public IConfig Create(string namespaceName)
        {
            var configRepository = _repositoryFactory.ConfigRepository(namespaceName);

            return new DefaultConfig(namespaceName, configRepository);
        }
    }
}
