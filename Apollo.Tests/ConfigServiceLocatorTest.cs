using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Internals;
using Com.Ctrip.Framework.Apollo.Util.Http;
using System.Threading.Tasks;
using Xunit;

namespace Apollo.Tests
{
    public class ConfigServiceLocatorTest
    {
        [Fact]
        public async Task MetaServerTest()
        {
            var options = new ApolloOptions
            {
                AppId = "apollo-client",
                MetaServer = "http://106.12.25.204:8080/"
            };

            var locator = new ConfigServiceLocator(new HttpUtil(options), options);

            var services = await locator.GetConfigServices().ConfigureAwait(false);

            Assert.Equal(1, services.Count);
            Assert.Equal(options.MetaServer, services[0].HomepageUrl);
        }

        [Fact]
        public async Task ConfigServerTest()
        {
            var options = new ApolloOptions
            {
                ConfigServer = new[] { "http://106.12.25.204:8080/" }
            };

            var locator = new ConfigServiceLocator(new HttpUtil(options), options);

            var services = await locator.GetConfigServices().ConfigureAwait(false);

            Assert.Equal(1, services.Count);
            Assert.Equal(options.MetaServer, services[0].HomepageUrl);
        }
    }
}
