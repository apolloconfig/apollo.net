using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Xunit;

namespace Aspire.Hosting.Apollo.Tests;

public class ApolloResourceBuilderExtensionsTests
{
    [Fact]
    public void AddApollo_CreatesApolloResource()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var apolloBuilder = builder.AddApollo("apollo", "testApp", "http://localhost:8080");

        // Assert
        Assert.NotNull(apolloBuilder);
        Assert.NotNull(apolloBuilder.Resource);
        Assert.IsType<ApolloResource>(apolloBuilder.Resource);
        Assert.Equal("apollo", apolloBuilder.Resource.Name);
        Assert.Equal("testApp", apolloBuilder.Resource.AppId);
        Assert.Equal("http://localhost:8080", apolloBuilder.Resource.MetaServer);
    }

    [Fact]
    public void AddApollo_WithNullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        IDistributedApplicationBuilder builder = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AddApollo("apollo"));
    }

    [Fact]
    public void AddApollo_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.AddApollo(""));
    }

    [Fact]
    public void AddApollo_WithNullAppIdAndMetaServer_CreatesResource()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var apolloBuilder = builder.AddApollo("apollo");

        // Assert
        Assert.NotNull(apolloBuilder);
        Assert.NotNull(apolloBuilder.Resource);
        Assert.Equal("apollo", apolloBuilder.Resource.Name);
        Assert.Null(apolloBuilder.Resource.AppId);
        Assert.Null(apolloBuilder.Resource.MetaServer);
    }

    [Fact]
    public void WithEnvironment_SetsEnvironment()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");

        // Act
        apolloBuilder.WithEnvironment("PRO");

        // Assert
        Assert.Equal("PRO", apolloBuilder.Resource.Env);
    }

    [Fact]
    public void WithCluster_SetsCluster()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");

        // Act
        apolloBuilder.WithCluster("production");

        // Assert
        Assert.Equal("production", apolloBuilder.Resource.Cluster);
    }

    [Fact]
    public void WithSecret_SetsSecret()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");

        // Act
        apolloBuilder.WithSecret("my-secret");

        // Assert
        Assert.Equal("my-secret", apolloBuilder.Resource.Secret);
    }

    [Fact]
    public void WithNamespaces_SetsNamespaces()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");

        // Act
        apolloBuilder.WithNamespaces("application", "database", "redis");

        // Assert
        Assert.NotNull(apolloBuilder.Resource.Namespaces);
        Assert.Equal(3, apolloBuilder.Resource.Namespaces.Count());
        Assert.Contains("application", apolloBuilder.Resource.Namespaces);
        Assert.Contains("database", apolloBuilder.Resource.Namespaces);
        Assert.Contains("redis", apolloBuilder.Resource.Namespaces);
    }

    [Fact]
    public void WithTimeout_SetsTimeout()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");

        // Act
        apolloBuilder.WithTimeout(10000);

        // Assert
        Assert.Equal(10000, apolloBuilder.Resource.Timeout);
    }

    [Fact]
    public void WithRefreshInterval_SetsRefreshInterval()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");

        // Act
        apolloBuilder.WithRefreshInterval(60000);

        // Assert
        Assert.Equal(60000, apolloBuilder.Resource.RefreshInterval);
    }

    [Fact]
    public void WithConfigServer_SetsConfigServers()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");

        // Act
        apolloBuilder.WithConfigServer("http://server1:8080", "http://server2:8080");

        // Assert
        Assert.NotNull(apolloBuilder.Resource.ConfigServer);
        Assert.Equal(2, apolloBuilder.Resource.ConfigServer.Count);
        Assert.Contains("http://server1:8080", apolloBuilder.Resource.ConfigServer);
        Assert.Contains("http://server2:8080", apolloBuilder.Resource.ConfigServer);
    }

    [Fact]
    public void WithMeta_SetsMeta()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var apolloBuilder = builder.AddApollo("apollo");
        var meta = new Dictionary<string, string>
        {
            ["DEV"] = "http://dev:8080",
            ["PRO"] = "http://prod:8080"
        };

        // Act
        apolloBuilder.WithMeta(meta);

        // Assert
        Assert.NotNull(apolloBuilder.Resource.Meta);
        Assert.Equal(2, apolloBuilder.Resource.Meta.Count);
        Assert.Equal("http://dev:8080", apolloBuilder.Resource.Meta["DEV"]);
        Assert.Equal("http://prod:8080", apolloBuilder.Resource.Meta["PRO"]);
    }

    [Fact]
    public void FluentConfiguration_WorksCorrectly()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        var apolloBuilder = builder.AddApollo("apollo")
            .WithAppId("TestApp")
            .WithMetaServer("http://localhost:8080")
            .WithEnvironment("PRO")
            .WithCluster("default")
            .WithSecret("test-secret")
            .WithNamespaces("application", "database")
            .WithTimeout(5000)
            .WithRefreshInterval(300000);

        // Assert
        var resource = apolloBuilder.Resource;
        Assert.Equal("TestApp", resource.AppId);
        Assert.Equal("http://localhost:8080", resource.MetaServer);
        Assert.Equal("PRO", resource.Env);
        Assert.Equal("default", resource.Cluster);
        Assert.Equal("test-secret", resource.Secret);
        Assert.Equal(2, resource.Namespaces?.Count());
        Assert.Equal(5000, resource.Timeout);
        Assert.Equal(300000, resource.RefreshInterval);
    }
}
