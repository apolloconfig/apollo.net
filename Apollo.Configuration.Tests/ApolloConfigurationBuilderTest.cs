using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Xunit;

namespace Apollo.Configuration.Tests
{
    public class ApolloConfigurationBuilderTest
    {
        [Fact]
        public void RepeatNamespaceTest()
        {
            var builder = new ConfigurationBuilder().AddApollo(new ApolloOptions { AppId = "a", MetaServer = "http://localhost:8080" });

            builder.AddDefault();
            builder.AddNamespace("test");
            builder.AddNamespace(ConfigConsts.NamespaceApplication);

            var sources = builder.Sources.OfType<ApolloConfigurationProvider>().ToArray();
            Assert.Equal(2, sources.Length);
            Assert.Equal(builder.ConfigRepositoryFactory.GetConfigRepository("test"), sources[0].ConfigRepository);
            Assert.Equal(builder.ConfigRepositoryFactory.GetConfigRepository(ConfigConsts.NamespaceApplication), sources[1].ConfigRepository);
        }
    }
}
