using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Xunit;

namespace Apollo.ConfigurationManager.Tests;

public class PropertyPlaceholderHelperTest
{
    [Fact]
    public void ResolvePlaceholders_ResolvesNullAndEmpty()
    {
        // Arrange
        var text = "foo=${foo??a},bar=${foo||b}";

        var dic1 = new Dictionary<string, string>
        {
            {"foo", ""}
        };

        // Act and Assert
        var result = new TestConfig(dic1).ResolvePlaceholders(text);

        Assert.Equal("foo=,bar=b", result);
    }

    [Fact]
    public void ResolvePlaceholders_ResolvesSinglePlaceholder()
    {
        // Arrange
        var text = "foo=${foo}";

        var dic1 = new Dictionary<string, string>
        {
            { "foo", "bar" }
        };

        // Act and Assert
        var result = new TestConfig(dic1).ResolvePlaceholders(text);

        Assert.Equal("foo=bar", result);
    }

    [Fact]
    public void ResolvePlaceholders_ResolvesMultiplePlaceholders()
    {
        // Arrange
        var text = "foo=${foo},bar=${bar}";

        var dic1 = new Dictionary<string, string>
        {
            { "foo", "bar" },
            { "bar", "baz" }
        };

        // Act and Assert
        var result = new TestConfig(dic1).ResolvePlaceholders(text);

        Assert.Equal("foo=bar,bar=baz", result);
    }

    [Fact]
    public void ResolvePlaceholders_ResolvesMultipleRecursivePlaceholders()
    {
        // Arrange
        var text = "foo=${bar}";

        var dic1 = new Dictionary<string, string>
        {
            { "bar", "${baz}" },
            { "baz", "bar" }
        };

        // Act and Assert
        var result = new TestConfig(dic1).ResolvePlaceholders(text);

        Assert.Equal("foo=bar", result);
    }

    [Fact]
    public void ResolvePlaceholders_ResolvesMultipleRecursiveInPlaceholders()
    {
        // Arrange
        var text1 = "foo=${b${inner}}";

        var dic1 = new Dictionary<string, string>
        {
            { "bar", "bar" },
            { "inner", "ar" }
        };

        var config1 = new TestConfig(dic1);

        var text2 = "${top}";

        var dic2 = new Dictionary<string, string>
            {
                {"top", "${child}+${child}"},
                {"child", "${${differentiator}.grandchild}"},
                {"differentiator", "first"},
                {"first.grandchild", "actualValue"}
            };

        var config2 = new TestConfig(dic2);

        // Act and Assert
        var result1 = config1.ResolvePlaceholders(text1);
        Assert.Equal("foo=bar", result1);
        var result2 = config2.ResolvePlaceholders(text2);
        Assert.Equal("actualValue+actualValue", result2);
    }

    [Fact]
    public void ResolvePlaceholders_UnresolvedPlaceholderIsIgnored()
    {
        // Arrange
        var text = "foo=${foo},bar=${bar}";

        var dic1 = new Dictionary<string, string>
        {
            { "foo", "bar" }
        };

        // Act and Assert
        var result = new TestConfig(dic1).ResolvePlaceholders(text);
        Assert.Equal("foo=bar,bar=${bar}", result);
    }

    [Fact]
    public void ResolvePlaceholders_ResolvesArrayRefPlaceholder()
    {
        // Arrange
        var json1 = @"
{
    ""vcap"": {
        ""application"": {
          ""application_id"": ""fa05c1a9-0fc1-4fbd-bae1-139850dec7a3"",
          ""application_name"": ""my-app"",
          ""application_uris"": [
            ""my-app.10.244.0.34.xip.io""
          ],
          ""application_version"": ""fb8fbcc6-8d58-479e-bcc7-3b4ce5a7f0ca"",
          ""limits"": {
            ""disk"": 1024,
            ""fds"": 16384,
            ""mem"": 256
          },
          ""name"": ""my-app"",
          ""space_id"": ""06450c72-4669-4dc6-8096-45f9777db68a"",
          ""space_name"": ""my-space"",
          ""uris"": [
            ""my-app.10.244.0.34.xip.io"",
            ""my-app2.10.244.0.34.xip.io""
          ],
          ""users"": null,
          ""version"": ""fb8fbcc6-8d58-479e-bcc7-3b4ce5a7f0ca""
        }
    }
}";

        var config = new TestConfig(new JsonConfigAdapter().GetProperties(json1));

        var text = "foo=${vcap:application:uris[1]}";

        // Act and Assert
        var result = config.ResolvePlaceholders(text);
        Assert.Equal("foo=my-app2.10.244.0.34.xip.io", result);
    }
#if DEBUG
    [Fact]
    public void GetResolvedConfigurationPlaceholders_ReturnsValues_WhenResolved()
    {
        // arrange
        var dic1 = new Dictionary<string, string>
        {
            { "foo", "${bar}" },
            { "bar", "baz" }
        };

        // act
        var resolved = new TestConfig(dic1).GetResolvedConfigurationPlaceholders().ToArray();

        // assert
        Assert.Contains(resolved, f => f.Key == "foo");
        Assert.DoesNotContain(resolved, f => f.Key == "bar");
        Assert.Equal("baz", resolved.First(k => k.Key == "foo").Value);
    }

    [Fact]
    public void GetResolvedConfigurationPlaceholders_ReturnsEmpty_WhenUnResolved()
    {
        var dic1 = new Dictionary<string, string>
        {
            { "foo", "${bar}" }
        };

        // act
        var resolved = new TestConfig(dic1).GetResolvedConfigurationPlaceholders().ToArray();

        // assert
        Assert.Contains(resolved, f => f.Key == "foo");
        Assert.Equal(string.Empty, resolved.First(k => k.Key == "foo").Value);
    }
#endif
    [Fact]
    public void Circular_Placeholder()
    {
        var dic1 = new Dictionary<string, string>
        {
            { "top", "${child}+${child}" },
            { "child", "${differentiator}" },
            { "differentiator", "${top}abc" }
        };

        // Act and Assert
        Assert.Throws<ArgumentException>(() => new TestConfig(dic1).ResolvePlaceholders("a/${top}"));
    }
}
