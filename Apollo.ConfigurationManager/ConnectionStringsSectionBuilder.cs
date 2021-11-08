using System.Collections.Specialized;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo
{
    public class ConnectionStringsSectionBuilder : ApolloConfigurationBuilder
    {
        private string? _keyPrefix;

        private string? _defaultProviderName;

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _keyPrefix = config["keyPrefix"];

            _defaultProviderName = config["defaultProviderName"] ?? "System.Data.SqlClient";
        }

        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            if (configSection is not ConnectionStringsSection section) return base.ProcessConfigurationSection(configSection);

            var connectionStrings = section.ConnectionStrings;

            lock (this)
            {
                var config = GetConfig();

                if (string.IsNullOrEmpty(_keyPrefix)) _keyPrefix = configSection.SectionInformation.Name;

                foreach (var name in config.GetChildren(_keyPrefix!))
                {
                    var connectionName = name.Name;

                    if (!config.TryGetProperty($"{_keyPrefix}:{connectionName}:ConnectionString", out var connectionString) &&
                        !config.TryGetProperty($"{_keyPrefix}:{connectionName}", out connectionString) ||
                        string.IsNullOrWhiteSpace(connectionString)) continue;

                    connectionStrings.Remove(connectionName);

                    config.TryGetProperty($"{_keyPrefix}:{connectionName}:ProviderName", out var providerName);

                    connectionStrings.Add(new ConnectionStringSettings(connectionName, connectionString, providerName ?? _defaultProviderName));
                }
            }

            return base.ProcessConfigurationSection(configSection);
        }
    }
}
