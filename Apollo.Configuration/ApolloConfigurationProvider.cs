using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IConfigurationSource
    {
        private readonly string _sectionKey;
        private readonly IConfigRepository _configRepository;
        private readonly Task _initializeTask;

        public ApolloConfigurationProvider(string sectionKey, IConfigRepository configRepository)
        {
            _sectionKey = sectionKey;
            _configRepository = configRepository;

            if (SynchronizationContext.Current == null)
                _initializeTask = _configRepository.Initialize();
            else
            {
                AsyncHelper.RunSync(_configRepository.Initialize);

                _initializeTask = Task.CompletedTask;
            }
        }

        public override void Load()
        {
            _initializeTask.GetAwaiter().GetResult();

            _configRepository.AddChangeListener(this);

            SetData(_configRepository.GetConfig());
        }

        private void SetData(Properties properties)
        {
            Data = string.IsNullOrEmpty(_sectionKey) || properties.Source == null ? properties.Source : new Dictionary<string, string>(properties.Source.ToDictionary(kv => $"{_sectionKey}{ConfigurationPath.KeyDelimiter}{kv.Key}", kv => kv.Value), StringComparer.OrdinalIgnoreCase);
        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            SetData(newProperties);

            OnReload();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
    }
}
