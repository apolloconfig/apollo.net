using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Util.Http;
using System;
using System.Collections.Concurrent;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class ConfigRepositoryFactory
    {
        private readonly ConcurrentDictionary<string, IConfigRepository> _configRepositories = new ConcurrentDictionary<string, IConfigRepository>();

        public IApolloOptions Options { get; }

        public ConfigRepositoryFactory(IApolloOptions options) => Options = options;

        public IConfigRepository GetConfigRepository(string @namespace) =>
            _configRepositories.GetOrAdd(@namespace, CreateConfigRepository);

        private IConfigRepository CreateConfigRepository(string @namespace)
        {
            if (Env.Local.Equals(Options.ApolloEnv))
            {
                Console.WriteLine("==== Apollo is in local mode! Won\'t pull configs from remote server! ====");
                return new LocalFileConfigRepository(@namespace, Options);
            }

            var http = new HttpUtil(Options);
            var locator = new ConfigServiceLocator(http, Options);
            var pollService = new RemoteConfigLongPollService(locator, http, Options);

            return new LocalFileConfigRepository(@namespace, Options, new RemoteConfigRepository(@namespace, Options, http, locator, pollService));
        }
    }
}
