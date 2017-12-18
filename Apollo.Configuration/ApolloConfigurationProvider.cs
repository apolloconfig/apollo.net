using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Internals;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IConfigurationSource
    {
        private readonly string _sectionKey;
        private readonly IConfigRepository _configRepository;

        public ApolloConfigurationProvider(string sectionKey, IConfigRepository configRepository)
        {
            _sectionKey = sectionKey;
            _configRepository = configRepository;
        }

        public override void Load()
        {
            _configRepository.AddChangeListener(this);

            SetData(_configRepository.GetConfig());
        }

        private void SetData(Properties properties)
        {
            Data = string.IsNullOrEmpty(_sectionKey) ? properties.Source : properties.Source.ToDictionary(kv => $"{_sectionKey}{ConfigurationPath.KeyDelimiter}{kv.Key}", kv => kv.Value);
        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            SetData(newProperties);

            OnReload();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
    }
}
