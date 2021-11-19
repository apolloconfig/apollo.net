using Com.Ctrip.Framework.Apollo;
using Xunit;

namespace Apollo.ConfigurationManager.Tests;

public class ConfigExtensionsTest
{
    [Fact]
    public void GetChildren()
    {
        var config = new TestConfig(Create("a:B", "1"), Create("A:c", "2"), Create("A:C:d", "4"), Create("b", "3"));

        Assert.Equal(new[] { "a", "b" }, config.GetChildren("").Select(x => x.Name));
        Assert.Equal(new[] { "B", "c" }, config.GetChildren("a").Select(x => x.Name));
        Assert.Equal(new[] { "a:B", "A:c" }, config.GetChildren("a").Select(x => x.FullName));
    }

    [Fact]
    public void GetConnectionStrings_ConnectionString_Prior()
    {
        var config = new TestConfig(Create("a:b", "123"), Create("a:b:ConnectionString", "abc"));

        var connectionString = config.GetConnectionStrings("a", "sql").Single();

        Assert.Equal("b", connectionString.Name);
        Assert.Equal("abc", connectionString.ConnectionString);
        Assert.Equal("sql", connectionString.ProviderName);
    }

    [Fact]
    public void GetConnectionStrings_ProviderName()
    {
        var config = new TestConfig(Create("a:b", "123"), Create("a:b:ProviderName", "abc"));

        var connectionString = config.GetConnectionStrings("a", "sql").Single();

        Assert.Equal("123", connectionString.ConnectionString);
        Assert.Equal("abc", connectionString.ProviderName);
    }

    [Fact]
    public void GetConnectionStrings_No_Prefix()
    {
        var config = new TestConfig(Create("a", "123"));

        var connectionString = config.GetConnectionStrings("", "sql").Single();

        Assert.Equal("a", connectionString.Name);
        Assert.Equal("123", connectionString.ConnectionString);
    }

    [Fact]
    public void WithPrefix()
    {
        IConfig config = new TestConfig(Create("a:B", "1"), Create("A:c", "2"), Create("A:C:d", "4"), Create("b", "3"));

        Assert.Same(config, config.WithPrefix("   "));

        config = config.WithPrefix("a");

        Assert.Equal(new[] { "B", "c", "C:d" }, config.GetPropertyNames());
        Assert.Equal(new[] { "1", "2", "4" }, config.GetPropertyNames().Select(p => config.GetProperty(p, "")));
    }

    private static KeyValuePair<string, string> Create(string key, string value) => new(key, value);

    private class TestConfig : IConfig
    {
        private readonly IReadOnlyDictionary<string, string> _dict;

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
}

