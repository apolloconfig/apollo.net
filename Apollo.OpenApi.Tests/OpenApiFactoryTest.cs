using Com.Ctrip.Framework.Apollo.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Com.Ctrip.Framework.Apollo
{
    public class OpenApiFactoryTest
    {
        [Fact]
        public async Task ExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => new OpenApiFactory(new OpenApiOptions()));

            try
            {
                await new OpenApiFactory(new OpenApiOptions
                {
                    PortalUrl = new Uri("http://106.54.227.205:8070"),
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
                await new OpenApiFactory(new OpenApiOptions
                {
                    PortalUrl = new Uri("http://106.54.227.205:8070"),
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
                await new OpenApiFactory(new OpenApiOptions
                {
                    PortalUrl = new Uri("http://106.54.227.205:8070"),
                    Token = "19419f7d3e5a1b0b0cfe3e238b36e09718fb8e94"
                })
                    .CreateNamespaceClient("apollo-client", "PRO", "test", "test")
                    .GetNamespaceInfo()
                    .ConfigureAwait(false);
            }
            catch (ApolloOpenApiException e)
            {
                Assert.Equal(HttpStatusCode.InternalServerError, e.Status);
            }

            try
            {
                await new OpenApiFactory(new OpenApiOptions
                {
                    PortalUrl = new Uri("http://106.54.227.205:8070"),
                    Token = "19419f7d3e5a1b0b0cfe3e238b36e09718fb8e94"
                })
                    .CreateNamespaceClient("apollo-client", "DEV", "test", "test")
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
                new OpenApiFactory(new OpenApiOptions
                {
                    PortalUrl = new Uri("http://localhost:8070"),
                    Token = "e16e5cd903fd0c97a116c873b448544b9d086de9"
                })
            };

            var services = new ServiceCollection();

            services.Configure<OpenApiOptions>(options =>
                {
                    options.PortalUrl = new Uri("http://localhost:8070");
                    options.Token = "e16e5cd903fd0c97a116c873b448544b9d086de9";
                })
                .AddApolloOpenApi();

            var provider = services.BuildServiceProvider();

            yield return new object[] { provider.GetRequiredService<IOpenApiFactory>() };
        }
    }
}
