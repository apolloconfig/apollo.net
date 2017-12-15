using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Util.Http;
using System;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class ConfigRepositoryFactory
    {
        public IApolloOptions Options { get; }

        public ConfigRepositoryFactory(IApolloOptions options) => Options = options;

        public IConfigRepository ConfigRepository(string @namespace)
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
