using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Internals;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Com.Ctrip.Framework.Apollo.Core;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IConfigurationSource
    {
        private readonly string _namespace;
        private readonly IConfigRepository _configRepository;

        public ApolloConfigurationProvider(string @namespace, ConfigRepositoryFactory repositoryFactory)
        {
            _namespace = @namespace;
            _configRepository = repositoryFactory.ConfigRepository(@namespace ?? ConfigConsts.NamespaceApplication);
        }

        public override void Load()
        {
            _configRepository.AddChangeListener(this);

            SetData(_configRepository.GetConfig());
        }

        private void SetData(Properties properties)
        {
            Data = string.IsNullOrWhiteSpace(_namespace) ? properties.Source : properties.Source.ToDictionary(kv => $"{_namespace}{ConfigurationPath.KeyDelimiter}{kv.Key}", kv => kv.Value);
        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            SetData(newProperties);

            OnReload();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
    }
}
