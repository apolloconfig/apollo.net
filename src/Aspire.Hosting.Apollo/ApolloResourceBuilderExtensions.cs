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

        var resource = new ApolloResource(name)
        {
            AppId = appId,
            MetaServer = metaServer
        };
        
        return builder.AddResource(resource);
    }

    /// <summary>
    /// Sets the Apollo application ID.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="appId">The Apollo application ID.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithAppId(
        this IResourceBuilder<ApolloResource> builder,
        string appId)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(appId);

        builder.Resource.AppId = appId;
        return builder;
    }

    /// <summary>
    /// Sets the Apollo meta server URL.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="metaServer">The Apollo meta server URL.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithMetaServer(
        this IResourceBuilder<ApolloResource> builder,
        string metaServer)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(metaServer);

        builder.Resource.MetaServer = metaServer;
        return builder;
    }

    /// <summary>
    /// Sets the Apollo environment (e.g., DEV, FAT, UAT, PRO).
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="env">The environment name.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithEnvironment(
        this IResourceBuilder<ApolloResource> builder,
        string env)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(env);

        builder.Resource.Env = env;
        return builder;
    }

    /// <summary>
    /// Sets the cluster name for Apollo configuration.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="cluster">The cluster name.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithCluster(
        this IResourceBuilder<ApolloResource> builder,
        string cluster)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(cluster);

        builder.Resource.Cluster = cluster;
        return builder;
    }

    /// <summary>
    /// Sets the data center info.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="dataCenter">The data center name.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithDataCenter(
        this IResourceBuilder<ApolloResource> builder,
        string dataCenter)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(dataCenter);

        builder.Resource.DataCenter = dataCenter;
        return builder;
    }

    /// <summary>
    /// Sets the access secret for authentication.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="secret">The access secret.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithSecret(
        this IResourceBuilder<ApolloResource> builder,
        string secret)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(secret);

        builder.Resource.Secret = secret;
        return builder;
    }

    /// <summary>
    /// Sets the direct config server URLs to skip meta service discovery.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="configServers">The config server URLs.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithConfigServer(
        this IResourceBuilder<ApolloResource> builder,
        params string[] configServers)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configServers);

        builder.Resource.ConfigServer = configServers;
        return builder;
    }

    /// <summary>
    /// Sets the namespaces to load from Apollo.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="namespaces">The namespace names.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithNamespaces(
        this IResourceBuilder<ApolloResource> builder,
        params string[] namespaces)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(namespaces);

        builder.Resource.Namespaces = namespaces;
        return builder;
    }

    /// <summary>
    /// Sets the timeout for HTTP requests in milliseconds.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithTimeout(
        this IResourceBuilder<ApolloResource> builder,
        int timeout)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Resource.Timeout = timeout;
        return builder;
    }

    /// <summary>
    /// Sets the refresh interval in milliseconds.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="refreshInterval">The refresh interval in milliseconds.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithRefreshInterval(
        this IResourceBuilder<ApolloResource> builder,
        int refreshInterval)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Resource.RefreshInterval = refreshInterval;
        return builder;
    }

    /// <summary>
    /// Sets the local cache directory path.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="localCacheDir">The local cache directory path.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithLocalCacheDir(
        this IResourceBuilder<ApolloResource> builder,
        string localCacheDir)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(localCacheDir);

        builder.Resource.LocalCacheDir = localCacheDir;
        return builder;
    }

    /// <summary>
    /// Sets environment-specific meta server URLs.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="meta">Dictionary of environment names to meta server URLs.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithMeta(
        this IResourceBuilder<ApolloResource> builder,
        IDictionary<string, string> meta)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(meta);

        builder.Resource.Meta = meta;
        return builder;
    }

    /// <summary>
    /// Sets the configuration label.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="label">The configuration label.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<ApolloResource> WithLabel(
        this IResourceBuilder<ApolloResource> builder,
        string label)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(label);

        builder.Resource.Label = label;
        return builder;
    }

    /// <summary>
    /// Adds a binding to an Apollo configuration resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="apollo">The Apollo resource builder to reference.</param>
    /// <param name="appId">Optional Apollo application ID to override.</param>
    /// <param name="metaServer">Optional Apollo meta server URL to override.</param>
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

        var resource = apollo.Resource;

        // Set all Apollo configuration options as environment variables
        SetEnvironmentVariable(builder, "Apollo__AppId", appId ?? resource.AppId);
        SetEnvironmentVariable(builder, "Apollo__MetaServer", metaServer ?? resource.MetaServer);
        SetEnvironmentVariable(builder, "Apollo__Env", resource.Env);
        SetEnvironmentVariable(builder, "Apollo__Cluster", resource.Cluster);
        SetEnvironmentVariable(builder, "Apollo__DataCenter", resource.DataCenter);
        SetEnvironmentVariable(builder, "Apollo__Secret", resource.Secret);
        SetEnvironmentVariable(builder, "Apollo__LocalCacheDir", resource.LocalCacheDir);
        SetEnvironmentVariable(builder, "Apollo__Label", resource.Label);

        if (resource.Timeout.HasValue)
        {
            builder.WithEnvironment("Apollo__Timeout", resource.Timeout.Value.ToString());
        }

        if (resource.RefreshInterval.HasValue)
        {
            builder.WithEnvironment("Apollo__RefreshInterval", resource.RefreshInterval.Value.ToString());
        }

        if (resource.ConfigServer != null && resource.ConfigServer.Count > 0)
        {
            for (int i = 0; i < resource.ConfigServer.Count; i++)
            {
                builder.WithEnvironment($"Apollo__ConfigServer__{i}", resource.ConfigServer.ElementAt(i));
            }
        }

        var actualNamespaces = namespaces ?? resource.Namespaces?.ToArray();
        if (actualNamespaces != null && actualNamespaces.Length > 0)
        {
            for (int i = 0; i < actualNamespaces.Length; i++)
            {
                builder.WithEnvironment($"Apollo__Namespaces__{i}", actualNamespaces[i]);
            }
        }

        if (resource.Meta != null && resource.Meta.Count > 0)
        {
            foreach (var kvp in resource.Meta)
            {
                builder.WithEnvironment($"Apollo__Meta__{kvp.Key}", kvp.Value);
            }
        }

        return builder;
    }

    private static void SetEnvironmentVariable<TDestination>(
        IResourceBuilder<TDestination> builder,
        string key,
        string? value)
        where TDestination : IResourceWithEnvironment
    {
        if (!string.IsNullOrEmpty(value))
        {
            builder.WithEnvironment(key, value);
        }
    }
}
