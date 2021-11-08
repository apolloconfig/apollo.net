using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Xunit;

namespace Apollo.ConfigurationManager.Tests
{
    public class XmlExtensionsTest
    {
#if NET471_OR_GREATER
        [Theory]
        [InlineData("<fpSection isMaster=\"true\"><abc>def</abc></fpSection>",
            "<fpSection isMaster=\"false\"><settings><a key=\"AlertMailDisabled\" value=\"0\" /><b key=\"FPAlertRemark\" value=\"\" /></settings></fpSection>",
            "<fpSection isMaster=\"false\"><abc>def</abc><settings><a key=\"AlertMailDisabled\" value=\"0\" /><b key=\"FPAlertRemark\" value=\"\" /></settings></fpSection>")]
        public void BindTest(string originalXml, string newXml, string resultXml)
        {
            var doc = new XmlDocument();

            doc.LoadXml(originalXml);

            XmlNode? node = doc.DocumentElement;

            node?.Bind(new FakeConfig(new XmlConfigAdapter().GetProperties($"<xml>{newXml}</xml>")), node.Name);

            Assert.Equal(resultXml, node?.OuterXml);
        }
#endif
        private class FakeConfig : IConfig
        {
            private readonly Properties _data;

            public event ConfigChangeEvent ConfigChanged = default!;

            public FakeConfig(Properties data) => _data = data;

            public IEnumerable<string> GetPropertyNames() => _data.GetPropertyNames();

            public bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) => _data.TryGetProperty(key, out value);
        }
    }
}
