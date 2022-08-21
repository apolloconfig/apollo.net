using Com.Ctrip.Framework.Apollo.Enums;
using System.Collections.ObjectModel;

namespace Com.Ctrip.Framework.Apollo;

public interface IApolloOptions : IDisposable
{
    string AppId { get; }
    /// <summary>
    /// Get the data center info for the current application.
    /// </summary>
    /// <returns> the current data center, null if there is no such info. </returns>
    string? DataCenter { get; }

    /// <summary>
    /// Get the cluster name for the current application.
    /// </summary>
    /// <returns> the cluster name, or "default" if not specified </returns>
    string Cluster { get; }

    /// <summary>
    /// Get the current environment.
    /// </summary>
    /// <returns> the env </returns>
    Env Env { get; }

    string LocalIp { get; }

    string? MetaServer { get; }

    string? Secret { get; }
#if NET40
    ReadOnlyCollection<string>? ConfigServer { get; }
#else
    IReadOnlyCollection<string>? ConfigServer { get; }
#endif
    /// <summary>Load config timeout. ms</summary>
    int Timeout { get; }

    /// <summary>Refresh interval. ms</summary>
    int RefreshInterval { get; }

    string? LocalCacheDir { get; }

    HttpMessageHandler HttpMessageHandler { get; }

    ICacheFileProvider CacheFileProvider { get; }

    /// <summary>ms, default value is 30000. If the config fails to be obtained at startup and there is no local cache, wait until successful or timeout.</summary>
    int StartupTimeout { get; }
#if NET40
    ReadOnlyCollection<string>? SpecialDelimiter { get; }
#else
    IReadOnlyCollection<string>? SpecialDelimiter { get; }
#endif
}
