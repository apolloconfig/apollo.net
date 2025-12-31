using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding Apollo configuration resources to an <see cref="IDistributedApplicationBuilder"/>.
/// </summary>
public static class ApolloResourceBuilderExtensions
{
    /// <summary>
    /// Adds an Apollo configuration server resource to the application.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="appId">The Apollo application ID.</param>
    /// <param name="metaServer">The Apollo meta server URL.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> AddApollo(
        this IDistributedApplicationBuilder builder,
        string name,
        string? appId = null,
        string? metaServer = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new ApolloResource(name, appId, metaServer);
        
        return builder.AddResource(resource);
    }

    /// <summary>
    /// Adds a binding to an Apollo configuration resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="apollo">The Apollo resource builder to reference.</param>
    /// <param name="appId">The Apollo application ID to bind to.</param>
    /// <param name="metaServer">The Apollo meta server URL to bind to.</param>
    /// <param name="namespaces">Optional list of namespaces to use.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        IResourceBuilder<ApolloResource> apollo,
        string? appId = null,
        string? metaServer = null,
        string[]? namespaces = null)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(apollo);

        var actualAppId = appId ?? apollo.Resource.AppId;
        var actualMetaServer = metaServer ?? apollo.Resource.MetaServer;

        if (!string.IsNullOrEmpty(actualAppId))
        {
            builder.WithEnvironment("Apollo__AppId", actualAppId);
        }

        if (!string.IsNullOrEmpty(actualMetaServer))
        {
            builder.WithEnvironment("Apollo__MetaServer", actualMetaServer);
        }

        if (namespaces != null && namespaces.Length > 0)
        {
            builder.WithEnvironment("Apollo__Namespaces", string.Join(",", namespaces));
        }

        return builder;
    }
}
