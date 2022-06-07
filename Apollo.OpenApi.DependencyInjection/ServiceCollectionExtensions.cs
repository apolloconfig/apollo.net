using Com.Ctrip.Framework.Apollo.OpenApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApolloOpenApi(this IServiceCollection services) =>
        services.AddApolloOpenApi(ServiceLifetime.Singleton);

    public static IServiceCollection AddApolloOpenApi(this IServiceCollection services, ServiceLifetime lifetime,
        string handlerName = "")
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddHttpClient();

        if (lifetime == ServiceLifetime.Singleton)
            services.TryAddSingleton<IOpenApiFactory>(provider => new OpenApiFactory(
                provider.GetRequiredService<IOptions<OpenApiOptions>>().Value,
                () => provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(handlerName), false));
        else
            services.TryAdd(new ServiceDescriptor(typeof(IOpenApiFactory), provider => new OpenApiFactory(
                    provider.GetRequiredService<IOptionsSnapshot<OpenApiOptions>>().Value,
                    () => provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(handlerName), false),
                lifetime));

        return services;
    }
}
