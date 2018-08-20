#if CONFIGURATIONBUILDER
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace Com.Ctrip.Framework.Apollo
{
    public class ConnectionStringsSectionBuilder : ApolloConfigurationBuilder
    {
        private string _defaultProviderName;
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _defaultProviderName = config["defaultProviderName"] ?? "System.Data.SqlClient";
        }

        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            if (configSection is ConnectionStringsSection section)
            {
                var connectionStrings = section.ConnectionStrings;
                lock (this)
                {
                    var config = GetConfig();
                    var names = config.GetPropertyNames()
                        .Where(name => name.StartsWith("ConnectionStrings:", StringComparison.OrdinalIgnoreCase))
                        .GroupBy(name => name.Substring(18).Split(':')[0])
                        .ToDictionary(group => group.Key, group => group.ToArray());

                    foreach (var name in names)
                    {
                        var connectionName = name.Key;
                        if (name.Value.Length == 1)
                        {
                            var connectionString = config.GetProperty(name.Value[0], null);
                            if (string.IsNullOrWhiteSpace(connectionString))
                                continue;

                            connectionStrings.Remove(connectionName);

                            connectionStrings.Add(new ConnectionStringSettings(connectionName, connectionString, _defaultProviderName));
                        }
                        else
                        {
                            var connectionString = config.GetProperty($"ConnectionStrings:{connectionName}:ConnectionString", config.GetProperty($"ConnectionStrings:{connectionName}", null));
                            if (string.IsNullOrWhiteSpace(connectionString))
                                continue;

                            var providerName = config.GetProperty($"ConnectionStrings:{connectionName}:ProviderName", null);

                            connectionStrings.Add(new ConnectionStringSettings(connectionName, connectionString, providerName ?? _defaultProviderName));
                        }
                    }
                }
            }

            return base.ProcessConfigurationSection(configSection);
        }
    }
}
#endif
