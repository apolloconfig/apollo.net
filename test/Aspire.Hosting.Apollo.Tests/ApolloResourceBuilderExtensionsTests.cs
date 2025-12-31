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
}
