using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Internals;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IConfigurationSource, IDisposable
    {
        internal string? SectionKey { get; }
        internal IConfigRepository ConfigRepository { get; }
        private Task? _initializeTask;

        public ApolloConfigurationProvider(string? sectionKey, IConfigRepository configRepository)
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
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in properties.GetPropertyNames())
            {
                if (string.IsNullOrEmpty(SectionKey))
                    data[key] = properties.GetProperty(key) ?? string.Empty;
                else
                    data[$"{SectionKey}{ConfigurationPath.KeyDelimiter}{key}"] = properties.GetProperty(key) ?? string.Empty;
            }

            Data = data;
        }

        void IRepositoryChangeListener.OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            SetData(newProperties);

            OnReload();
        }

        IConfigurationProvider IConfigurationSource.Build(IConfigurationBuilder builder) => this;

        public void Dispose() => ConfigRepository.RemoveChangeListener(this);
    }
}
