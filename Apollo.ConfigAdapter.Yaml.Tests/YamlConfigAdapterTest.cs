using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Enums;
using System;
using Xunit;

namespace Com.Ctrip.Framework.Apollo
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

            Assert.Null(properties.Source["environments:dev:url"]);
            Assert.Null(properties.Source["environments:dev:url2"]);
            Assert.Null(properties.Source["environments:dev:url3"]);
            Assert.Null(properties.Source["environments:dev:url4"]);
            Assert.Equal("`", properties.Source["environments:dev:url5"]);
            Assert.Equal("", properties.Source["environments:dev:name"]);
        }
    }
}
