using System.ServiceModel.Configuration;
using Xunit;

namespace Apollo.ConfigurationManager.Tests;

public class ConfigurationBuilderTest
{
#if NET471_OR_GREATER
    [Fact]
    public void AppSettingsSectionBuilderTest() =>
        Assert.Equal("00:00:30", System.Configuration.ConfigurationManager.AppSettings["Timeout"]);

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

    [Fact]
    public void CommonConfigurationBuilderTest()
    {
        var test = (TestConfigurationSection)System.Configuration.ConfigurationManager.GetSection("test");

        Assert.Equal(3, test.DefaultValue);

        Assert.Equal(TimeSpan.FromSeconds(30), test.Timeout);

        Assert.Equal(100, test.MaxValue);

        Assert.NotNull(test.Map);

        var element = test.Map["abc"];

        Assert.NotNull(element);

        Assert.Equal("abc", element.Key);
        Assert.Equal("123", element.Value);

        Assert.Null(test.Map["def"]);

        element = test.Map["defg"];

        Assert.NotNull(element);

        Assert.Equal("defg", element.Key);
        Assert.Equal("456", element.Value);

        Assert.NotNull(test.Element);

        Assert.Equal("jkl", test.Element.Name);
        Assert.Equal("789", test.Element.Value);
    }
#endif
}
