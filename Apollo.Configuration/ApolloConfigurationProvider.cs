using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Internals;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IConfigurationSource
    {
        internal string SectionKey { get; }
        internal IConfigRepository ConfigRepository { get; }
        private Task _initializeTask;

        public ApolloConfigurationProvider(string sectionKey, IConfigRepository configRepository)
        {
            SectionKey = sectionKey;
            ConfigRepository = configRepository;
            ConfigRepository.AddChangeListener(this);
            _initializeTask = ConfigRepository.Initialize();
        }

        public override void Load()
        {
            Interlocked.Exchange(ref _initializeTask, null)?.ConfigureAwait(false).GetAwaiter().GetResult();

            SetData(ConfigRepository.GetConfig());
        }

        protected virtual void SetData(Properties properties)
        {
            if (string.IsNullOrEmpty(SectionKey) || properties.Source == null || properties.Source.Count == 0)
                Data = properties.Source;
            else
            {
                var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var kv in properties.Source)
                    data[$"{SectionKey}{ConfigurationPath.KeyDelimiter}{kv.Key}"] = kv.Value;

                Data = data;
            }
        }

        void IRepositoryChangeListener.OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            SetData(newProperties);

            OnReload();
        }

        IConfigurationProvider IConfigurationSource.Build(IConfigurationBuilder builder) => this;
    }
}
