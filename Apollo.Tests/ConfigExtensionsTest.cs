using Com.Ctrip.Framework.Apollo;
using System;
using System.Collections.Generic;
using Xunit;

namespace Apollo.Tests
{
    public class ConfigExtensionsTest
    {
        [Fact]
        public void Test()
        {
            var config = new FakeConfig(new Dictionary<string, string> { ["int"] = "3", ["double"] = "abc", ["array"] = "1,2,3", });

            Assert.Equal(3, config.GetProperty("int", new int?()));
            Assert.Equal(3d, config.GetProperty("double", 3d));

            var array = config.GetProperty("array", ",", Array.Empty<string>());
            Assert.NotNull(array);
            Assert.Equal(new []{"1", "2", "3" }, array);
        }

        private class FakeConfig : IConfig
        {
            private readonly IReadOnlyDictionary<string, string> _data;

            public event ConfigChangeEvent ConfigChanged = default!;

            public FakeConfig(IReadOnlyDictionary<string, string> data) => _data = data;

            public IEnumerable<string> GetPropertyNames() => _data.Keys;

            public bool TryGetProperty(string key, out string value) => _data.TryGetValue(key, out value);
        }
    }
}
