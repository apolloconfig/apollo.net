using Com.Ctrip.Framework.Apollo.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Com.Ctrip.Framework.Apollo.Spi
{
    public class DefaultConfigRegistry : IConfigRegistry
    {
        private static readonly Func<Action<LogLevel, string, Exception?>> Logger = () => LogManager.CreateLogger(typeof(DefaultConfigRegistry));
        private readonly IDictionary<string, IConfigFactory> _instances = new ConcurrentDictionary<string, IConfigFactory>();

        public void Register(string namespaceName, IConfigFactory factory)
        {
            if (_instances.ContainsKey(namespaceName))
            {
                Logger().Warn($"ConfigFactory({namespaceName}) is overridden by {factory.GetType()}!");
            }

            _instances[namespaceName] = factory;

        }

        public IConfigFactory GetFactory(string namespaceName)
        {
            _instances.TryGetValue(namespaceName, out var config);
            return config;
        }
    }
}
