using Com.Ctrip.Framework.Apollo.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Apollo.OpenApi.Tests;

public class OpenApiFactoryTest
{
    [Fact]
    public async Task ExceptionTest()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenApiFactory(new()));

        try
        {
            await new OpenApiFactory(new()
            {
                PortalUrl = new("http://81.68.181.139:8070"),
                Token = Guid.NewGuid().ToString("N")
            })
                .CreateAppClusterClient("abc").GetAppInfo()
                .ConfigureAwait(false);
        }
        catch (ApolloOpenApiException e)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, e.Status);
        }

        try
        {
            await new OpenApiFactory(new()
            {
                PortalUrl = new("http://81.68.181.139:8070"),
                Token = "19419f7d3e5a1b0b0cfe3e238b36e09718fb8e94"
            })
                .CreateAppClusterClient("abc").GetAppInfo()
                .ConfigureAwait(false);
        }
        catch (ApolloOpenApiException e)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, e.Status);
        }

        try
        {
            await new OpenApiFactory(new()
            {
                PortalUrl = new("http://81.68.181.139:8070"),
                Token = "19419f7d3e5a1b0b0cfe3e238b36e09718fb8e94"
            })
                .CreateNamespaceClient("apollo.net", "PRO", "test", "test")
                .GetNamespaceInfo()
                .ConfigureAwait(false);
        }
        catch (ApolloOpenApiException e)
        {
            Assert.Equal(HttpStatusCode.InternalServerError, e.Status);
        }

        try
        {
            await new OpenApiFactory(new()
            {
                PortalUrl = new("http://81.68.181.139:8070"),
                Token = "19419f7d3e5a1b0b0cfe3e238b36e09718fb8e94"
            })
                .CreateNamespaceClient("apollo.net", "DEV", "test", "test")
                .GetNamespaceInfo()
                .ConfigureAwait(false);
        }
        catch (ApolloOpenApiException e)
        {
            Assert.Equal(HttpStatusCode.Unauthorized, e.Status);
        }
    }

    [Theory, MemberData(nameof(CreateOpenApiFactory))]
    public void CreateOpenApiFactoryTest(IOpenApiFactory factory)
    {
        Assert.NotNull(factory.CreateAppClusterClient("test"));

        Assert.NotNull(factory.CreateNamespaceClient("test", "DEV"));
    }

    public static IEnumerable<object[]> CreateOpenApiFactory()
    {
        yield return new object[]
        {
            new OpenApiFactory(new()
            {
                PortalUrl = new("http://localhost:8070"),
                Token = "e16e5cd903fd0c97a116c873b448544b9d086de9"
            })
        };

        var services = new ServiceCollection();

        services.Configure<OpenApiOptions>(options =>
            {
                options.PortalUrl = new("http://localhost:8070");
                options.Token = "e16e5cd903fd0c97a116c873b448544b9d086de9";
            })
            .AddApolloOpenApi();

        var provider = services.BuildServiceProvider();

        yield return new object[] { provider.GetRequiredService<IOpenApiFactory>() };
    }
}
