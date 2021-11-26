namespace Com.Ctrip.Framework.Apollo;

internal class PlaceholderConfig : IConfig
{
    private readonly IConfig _config;

    public PlaceholderConfig(IConfig config) => _config = config;

    public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value)
    {
        if (!_config.TryGetProperty(key, out value)) return false;

        value = this.ResolvePlaceholders(value);

        return true;
    }

    public IEnumerable<string> GetPropertyNames() => _config.GetPropertyNames();

    public event ConfigChangeEvent? ConfigChanged
    {
        add => _config.ConfigChanged += value;
        remove => _config.ConfigChanged -= value;
    }
}
