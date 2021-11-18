namespace Com.Ctrip.Framework.Apollo;

internal static class ConfigExtensions
{
    public static IEnumerable<ConfigKey> GetChildren(this IConfig config, string keyPrefix)
    {
        keyPrefix += ":";

        var hash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var propertyName in config.GetPropertyNames())
        {
            if (!propertyName.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase)) continue;

            var index = propertyName.IndexOf(':', keyPrefix.Length);

            hash.Add(index > 0 ? propertyName.Substring(0, index) : propertyName);
        }

        return hash.Select(key => new ConfigKey(key.Substring(key.LastIndexOf(':') + 1), key));
    }

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
}