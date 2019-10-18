using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Enums;
using System;
using Xunit;

namespace Apollo.ConfigAdapter.Yaml.Tests
{
    public class YamlConfigAdapterTest
    {
        [Fact]
        public void RegisterTest()
        {
            YamlConfigAdapter.Register();

            Assert.True(ConfigAdapterRegister.TryGetAdapter(ConfigFileFormat.Yml, out var a));
            Assert.True(ConfigAdapterRegister.TryGetAdapter(ConfigFileFormat.Yaml, out var b));

            Assert.Equal(a, b);

            Assert.NotNull(a);
        }

        [Fact]
        public void ParseTest()
        {
            var properties = new YamlConfigAdapter().GetProperties(@"environments:
    dev:
        url: http://dev.bar.com
        name: Developer Setup
    prod:
        url: http://foo.bar.com
        name: My Cool App
my:
    servers:
        - dev.bar.com
        - foo.bar.com");

            Assert.NotNull(properties);

            Assert.Equal(6, properties.GetPropertyNames().Count);

            Assert.Equal("foo.bar.com", properties.GetProperty("my:servers:1"));
        }

        [Fact]
        public void DuplicateKeyTest() =>
            Assert.Throws<FormatException>(() => new YamlConfigAdapter().GetProperties(@"environments:
    dev:
        url: http://dev.bar.com
        name: Developer Setup
    Dev:
        url: http://foo.bar.com
        name: My Cool App"));

        [Fact]
        public void NullTest()
        {
            var properties = new YamlConfigAdapter().GetProperties(@"environments:
    dev:
        url: ~
        url2: null
        url3: Null
        url4: NULL
        url5: '`'
        name: ''");

            Assert.NotNull(properties);

            Assert.Equal(6, properties.GetPropertyNames().Count);

            Assert.Empty(properties.GetProperty("environments:dev:url"));
            Assert.Empty(properties.GetProperty("environments:dev:url2"));
            Assert.Empty(properties.GetProperty("environments:dev:url3"));
            Assert.Empty(properties.GetProperty("environments:dev:url4"));
            Assert.Equal("`", properties.GetProperty("environments:dev:url5"));
            Assert.Empty(properties.GetProperty("environments:dev:name"));
        }

        [Fact]
        public void MergeKeysTest()
        {
            var properties = new YamlConfigAdapter().GetProperties(@"merge:
  - &CENTER { x: 1, y: 2 }
  - &LEFT { x: 0, y: 2 }
  - &BIG { r: 10 }
  - &SMALL { r: 1 }

sample1:
    <<: *CENTER
    r: 10

sample2:
    << : [ *CENTER, *BIG ]
    other: haha

sample3:
    << : [ *CENTER, *BIG ]
    r: 100");

            Assert.NotNull(properties);

            Assert.Equal("1", properties.GetProperty("sample1:x"));
            Assert.Equal("10", properties.GetProperty("sample2:r"));
            Assert.Equal("100", properties.GetProperty("sample3:r"));
        }
    }
}
