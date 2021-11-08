using System;
using System.ServiceModel.Configuration;
using Xunit;

namespace Apollo.ConfigurationManager.Tests
{
    public class ConfigurationBuilderTest
    {
#if NET471_OR_GREATER
        [Fact]
        public void AppSettingsSectionBuilderTest() =>
            Assert.Equal("560", System.Configuration.ConfigurationManager.AppSettings["Timeout"]);

        [Fact]
        public void ConnectionStringsSectionBuilderTest() =>
            Assert.Equal("asdfasdf", System.Configuration.ConfigurationManager.ConnectionStrings["abc"]?.ConnectionString);

        [Fact]
        public void NodeReplaceSectionBuilderTest()
        {
            var client = (ClientSection)System.Configuration.ConfigurationManager.GetSection("system.serviceModel/client");

            Assert.Single(client.Endpoints);

            var endpoint = client.Endpoints[0];

            Assert.Equal(new Uri("http://localhost:1234"), endpoint.Address);
            Assert.Equal("test", endpoint.Name);
        }
#endif
    }
}
