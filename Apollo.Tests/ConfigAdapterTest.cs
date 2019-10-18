using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Enums;
using Moq;
using System.IO;
using Xunit;

namespace Apollo.Tests
{
    public class ConfigAdapterTest
    {
        [Fact]
        public void JsonTest()
        {
            var json = @"{""a"":3, ""b"":{""c"":[4]}}";

            var data = JsonConfigurationParser.Parse(json);

            Assert.NotNull(data);
            Assert.Equal(2, data.Count);
            Assert.Equal("3", data["a"]);
            Assert.Equal("4", data["b:c:0"]);
        }

        [Fact]
        public void XmlTest()
        {
            var xml = @"<setting a=""3"">
    <b><c>4</c></b>
</setting>";

            using var reader = new StringReader(xml);
            var data = XmlConfigurationParser.Read(reader);

            Assert.NotNull(data);
            Assert.Equal(2, data.Count);
            Assert.Equal("3", data["a"]);
            Assert.Equal("4", data["b:c"]);
        }

        [Fact]
        public void ConfigAdapterRegisterTest()
        {
            Assert.True(ConfigAdapterRegister.TryGetAdapter(ConfigFileFormat.Json, out var json));
            Assert.IsType<JsonConfigAdapter>(json);

            Assert.True(ConfigAdapterRegister.TryGetAdapter(ConfigFileFormat.Xml, out var xml));
            Assert.IsType<XmlConfigAdapter>(xml);

            var moq = new Mock<IConfigAdapter>();

            ConfigAdapterRegister.AddAdapter(ConfigFileFormat.Txt, moq.Object);
            Assert.True(ConfigAdapterRegister.TryGetAdapter(ConfigFileFormat.Txt, out var txt));
            Assert.Equal(moq.Object, txt);
        }
    }
}
