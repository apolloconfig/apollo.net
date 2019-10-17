using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Util.Http;
using System;
using System.Collections.Concurrent;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class ConfigRepositoryFactory : IConfigRepositoryFactory
    {
        private readonly HttpUtil _httpUtil;
        private readonly ConcurrentDictionary<string, IConfigRepository> _configRepositories = new ConcurrentDictionary<string, IConfigRepository>();
        private readonly IApolloOptions _options;

        public ConfigRepositoryFactory(IApolloOptions options, HttpUtil? httpUtil = null)
        {
            _options = options;
            _httpUtil = httpUtil ?? new HttpUtil(options);
        }

        public IConfigRepository GetConfigRepository(string @namespace) =>
            _configRepositories.GetOrAdd(@namespace, CreateConfigRepository);

        private IConfigRepository CreateConfigRepository(string @namespace)
        {
            if (Env.Local.Equals(_options.Env))
            {
                Console.WriteLine("==== Apollo is in local mode! Won\'t pull configs from remote server! ====");
                return new LocalFileConfigRepository(@namespace, _options);
            }

            var locator = new ConfigServiceLocator(_httpUtil, _options);
            var pollService = new RemoteConfigLongPollService(locator, _httpUtil, _options);

            return new LocalFileConfigRepository(@namespace, _options, new RemoteConfigRepository(@namespace, _options, _httpUtil, locator, pollService));
        }
    }
}
