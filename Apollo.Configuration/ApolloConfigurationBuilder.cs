using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo.Internals;

namespace Com.Ctrip.Framework.Apollo
{
    public interface IApolloConfigurationBuilder : IConfigurationBuilder
    {
        ConfigRepositoryFactory ConfigRepositoryFactory { get; }
    }

    internal class ApolloConfigurationBuilder : IApolloConfigurationBuilder
    {
        private readonly IConfigurationBuilder _builder;

        public ConfigRepositoryFactory ConfigRepositoryFactory { get; }

        public ApolloConfigurationBuilder(IConfigurationBuilder builder, ConfigRepositoryFactory configRepositoryFactory)
        {
            _builder = builder;

            ConfigRepositoryFactory = configRepositoryFactory;
        }

        public IConfigurationBuilder Add(IConfigurationSource source) => _builder.Add(source);

        public IConfigurationRoot Build() => _builder.Build();

        public IDictionary<string, object> Properties => _builder.Properties;
        public IList<IConfigurationSource> Sources => _builder.Sources;
    }
}
