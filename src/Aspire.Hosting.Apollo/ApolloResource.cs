using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

/// <summary>
/// Represents an Apollo configuration resource.
/// </summary>
/// <param name="name">The name of the resource.</param>
public class ApolloResource(string name) : Resource(name)
{
    /// <summary>
    /// Gets or sets the Apollo application ID.
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// Gets or sets the Apollo meta server URL.
    /// </summary>
    public string? MetaServer { get; set; }

    /// <summary>
    /// Gets or sets the Apollo environment (e.g., DEV, FAT, UAT, PRO).
    /// </summary>
    public string? Env { get; set; }

    /// <summary>
    /// Gets or sets the cluster name for the Apollo configuration.
    /// </summary>
    public string? Cluster { get; set; }

    /// <summary>
    /// Gets or sets the data center info.
    /// </summary>
    public string? DataCenter { get; set; }

    /// <summary>
    /// Gets or sets the access secret for authentication.
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Gets or sets the direct config server URLs (to skip meta service).
    /// </summary>
    public IReadOnlyCollection<string>? ConfigServer { get; set; }

    /// <summary>
    /// Gets or sets the namespaces to load.
    /// </summary>
    public IEnumerable<string>? Namespaces { get; set; }

    /// <summary>
    /// Gets or sets the timeout in milliseconds for HTTP requests.
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// Gets or sets the refresh interval in milliseconds.
    /// </summary>
    public int? RefreshInterval { get; set; }

    /// <summary>
    /// Gets or sets the local cache directory path.
    /// </summary>
    public string? LocalCacheDir { get; set; }

    /// <summary>
    /// Gets or sets environment-specific meta server URLs.
    /// </summary>
    public IDictionary<string, string>? Meta { get; set; }

    /// <summary>
    /// Gets or sets the configuration label.
    /// </summary>
    public string? Label { get; set; }
}
