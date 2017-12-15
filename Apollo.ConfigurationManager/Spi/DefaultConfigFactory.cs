using System.Threading.Tasks;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public class DefaultConfigFactory : IConfigFactory
    {
        private static readonly ConfigRepositoryFactory RepositoryFactory = new ConfigRepositoryFactory(new ConfigUtil());

        public IConfig Create(string namespaceName)
        {
            var configRepository = RepositoryFactory.ConfigRepository(namespaceName);

            return new DefaultConfig(namespaceName, configRepository);
        }
    }
}
