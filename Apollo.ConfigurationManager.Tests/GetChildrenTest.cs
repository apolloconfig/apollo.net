using Com.Ctrip.Framework.Apollo;
using Xunit;

namespace Apollo.ConfigurationManager.Tests;

public class GetChildrenTest
{
    [Fact]
    public void Test()
    {
        var config = new FakeConfig(new Dictionary<string, string>
        {
            { "a:B", "1" },
            { "A:c", "2" },
            { "A:C:d", "4" },
        });

        Assert.Equal(new[] { "B", "c" }, config.GetChildren("a").Select(x => x.Name));
        Assert.Equal(new[] { "a:B", "A:c" }, config.GetChildren("a").Select(x => x.FullName));
        Assert.Equal(new[] { "d" }, config.GetChildren("a:C").Select(x => x.Name));
    }

    private class FakeConfig : IConfig
    {
        private readonly IReadOnlyDictionary<string, string> _data;

        public event ConfigChangeEvent ConfigChanged = default!;

        public FakeConfig(IReadOnlyDictionary<string, string> data) => _data = data;

        public IEnumerable<string> GetPropertyNames() => _data.Keys;

        public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) => _data.TryGetValue(key, out value);
    }
}