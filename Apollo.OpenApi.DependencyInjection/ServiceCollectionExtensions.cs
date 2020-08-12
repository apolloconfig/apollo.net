using Com.Ctrip.Framework.Apollo.OpenApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApolloOpenApi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpClient()
                .TryAddSingleton<IOpenApiFactory>(provider => new OpenApiFactory(
                    provider.GetRequiredService<IOptions<OpenApiOptions>>().Value,
                    provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler));

            return services;
        }
    }
}
