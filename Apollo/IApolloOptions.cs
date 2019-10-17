using Com.Ctrip.Framework.Apollo.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo
{
    public interface IApolloOptions
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

        IReadOnlyCollection<string>? ConfigServer { get; }

        /// <summary>ms</summary>
        int Timeout { get; }

        /// <summary>ms</summary>
        int RefreshInterval { get; }

        string? LocalCacheDir { get; }

        Func<HttpMessageHandler>? HttpMessageHandlerFactory { get; }
    }
}
