using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Internals;
using Moq;
using Xunit;

namespace Apollo.Tests;

public class ConfigServiceLocatorTest
{
    [Fact]
    public async Task MetaServerTest()
    {
        var moq = new Mock<IApolloOptions>();
        moq.SetupGet(o => o.AppId).Returns("apollo.net");
        moq.SetupGet(o => o.MetaServer).Returns("http://81.68.181.139:8080/");
        moq.SetupGet(o => o.ConfigServer).Returns(Array.Empty<string>());
        moq.SetupGet(o => o.Timeout).Returns(5000);
        moq.SetupGet(o => o.HttpMessageHandler).Returns(new HttpClientHandler());

        var options = moq.Object;

        var locator = new ConfigServiceLocator(new(options), options);

        var services = await locator.GetConfigServices();

        Assert.Single(services);
        Assert.Equal(options.MetaServer, services[0].HomepageUrl);
    }

    [Fact]
    public async Task ConfigServerTest()
    {
        var moq = new Mock<IApolloOptions>();
        moq.SetupGet(o => o.ConfigServer).Returns(new[] { "http://81.68.181.139:8080/" });

        var options = moq.Object;

        var locator = new ConfigServiceLocator(new(options), options);

        var services = await locator.GetConfigServices();

        Assert.Single(services);
        Assert.Equal(options.ConfigServer?.FirstOrDefault(), services[0].HomepageUrl);
    }
}
