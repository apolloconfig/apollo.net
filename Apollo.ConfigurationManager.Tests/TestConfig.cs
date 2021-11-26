using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core.Utils;

namespace Apollo.ConfigurationManager.Tests;

public class TestConfig : IConfig
{
    private readonly IReadOnlyDictionary<string, string> _dict;

    public TestConfig(Properties properties)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var key in properties.GetPropertyNames()) dict[key] = properties.GetProperty(key) ?? "";

        _dict = dict;
    }

    public TestConfig(IEnumerable<KeyValuePair<string, string>> keyValues)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var kv in keyValues) dict[kv.Key] = kv.Value;

        _dict = dict;
    }

    public TestConfig(params KeyValuePair<string, string>[] keyValues)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var kv in keyValues) dict[kv.Key] = kv.Value;

        _dict = dict;
    }

    public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) =>
        _dict.TryGetValue(key, out value);

    public IEnumerable<string> GetPropertyNames() => _dict.Keys;

    public event ConfigChangeEvent ConfigChanged = default!;
}
