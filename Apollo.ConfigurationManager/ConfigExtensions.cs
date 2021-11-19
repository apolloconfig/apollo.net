using System.Configuration;

namespace Com.Ctrip.Framework.Apollo;

internal static class ConfigExtensions
{
    public static IEnumerable<ConfigKey> GetChildren(this IConfig config, string keyPrefix)
    {
        var hash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(keyPrefix))
        {
            foreach (var propertyName in config.GetPropertyNames())
            {
                var index = propertyName.IndexOf(':');

                hash.Add(index > 0 ? propertyName.Substring(0, index) : propertyName);
            }
        }
        else
        {
            keyPrefix += ":";

            foreach (var propertyName in config.GetPropertyNames())
            {
                if (!propertyName.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase)) continue;

                var index = propertyName.IndexOf(':', keyPrefix.Length);

                hash.Add(index > 0 ? propertyName.Substring(0, index) : propertyName);
            }
        }

        return hash.Select(key => new ConfigKey(key.Substring(key.LastIndexOf(':') + 1), key));
    }

    public static IEnumerable<ConnectionStringSettings> GetConnectionStrings(this IConfig config,
        string keyPrefix, string? defaultProviderName)
    {
        string keyPrefixAndColon;
        if (string.IsNullOrWhiteSpace(keyPrefix)) keyPrefixAndColon = keyPrefix = "";
        else keyPrefixAndColon = keyPrefix + ":";

        foreach (var name in config.GetChildren(keyPrefix))
        {
            var connectionName = name.Name;

            if (!config.TryGetProperty($"{keyPrefixAndColon}{connectionName}:ConnectionString", out var connectionString) &&
                !config.TryGetProperty($"{keyPrefixAndColon}{connectionName}", out connectionString) ||
                string.IsNullOrWhiteSpace(connectionString)) continue;

            config.TryGetProperty($"{keyPrefixAndColon}{connectionName}:ProviderName", out var providerName);

            yield return new ConnectionStringSettings(connectionName, connectionString, providerName ?? defaultProviderName);
        }
    }

    public static IConfig WithPrefix(this IConfig config, string? keyPrefix) =>
        string.IsNullOrWhiteSpace(keyPrefix) ? config : new KeyPrefixConfig(config, keyPrefix!);

    public struct ConfigKey
    {
        public ConfigKey(string name, string fullName)
        {
            Name = name;

            FullName = fullName;
        }

        public string Name { get; }

        public string FullName { get; }
    }

    private class KeyPrefixConfig : IConfig
    {
        private readonly IConfig _config;
        private readonly string _keyPrefix;

        public KeyPrefixConfig(IConfig config, string keyPrefix)
        {
            _config = config;

            _keyPrefix = keyPrefix + ":";
        }

        public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) =>
            _config.TryGetProperty(_keyPrefix + key, out value);

        public IEnumerable<string> GetPropertyNames() => _config.GetPropertyNames()
            .Where(propertyName => propertyName.Length > _keyPrefix.Length &&
                                   propertyName.StartsWith(_keyPrefix, StringComparison.OrdinalIgnoreCase))
            .Select(propertyName => propertyName.Substring(_keyPrefix.Length));

        public event ConfigChangeEvent? ConfigChanged
        {
            add => _config.ConfigChanged += value;
            remove => _config.ConfigChanged -= value;
        }
    }
}
