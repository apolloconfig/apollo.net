using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

/// <summary>
/// Represents an Apollo configuration resource.
/// </summary>
/// <param name="name">The name of the resource.</param>
/// <param name="appId">The Apollo application ID.</param>
/// <param name="metaServer">The Apollo meta server URL.</param>
public class ApolloResource(string name, string? appId, string? metaServer) : Resource(name)
{
    /// <summary>
    /// Gets the Apollo application ID.
    /// </summary>
    public string? AppId { get; } = appId;

    /// <summary>
    /// Gets the Apollo meta server URL.
    /// </summary>
    public string? MetaServer { get; } = metaServer;
}
