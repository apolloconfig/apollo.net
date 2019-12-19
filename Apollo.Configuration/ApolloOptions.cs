using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloOptions : IApolloOptions
    {
        /// <summary>
        /// Get the app id for the current application.
        /// </summary>
        /// <returns> the app id or ConfigConsts.NO_APPID_PLACEHOLDER if app id is not available</returns>
        private string _appId = ConfigConsts.NoAppidPlaceholder;
        private string? _dataCenter;
        private string? _cluster;
        private string? _metaServer;

        public string AppId
        {
            get => _appId;
            set
            {
                if (LocalCacheDir == null)
                    LocalCacheDir = Path.Combine(ConfigConsts.DefaultLocalCacheDir, value);

                _appId = value;
            }
        }

        /// <summary>
        /// Get the data center info for the current application.
        /// </summary>
        /// <returns> the current data center, null if there is no such info. </returns>
        public virtual string? DataCenter
        {
            get => _dataCenter;
            set
            {
                _dataCenter = value;

                if (string.IsNullOrEmpty(_cluster) && !string.IsNullOrEmpty(_dataCenter))
                    _cluster = _dataCenter;
            }
        }

        /// <summary>
        /// Get the cluster name for the current application.
        /// </summary>
        /// <returns> the cluster name, or "default" if not specified </returns>
        public virtual string Cluster { get => _cluster ?? ConfigConsts.ClusterNameDefault; set => _cluster = value; }

        /// <summary>Default Dev</summary>
        public virtual Env Env { get; set; } = Env.Dev;

        public virtual string LocalIp { get; set; } = NetworkInterfaceManager.HostIp;

        /// <summary>Default http://localhost:8080</summary>
        public virtual string? MetaServer
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_metaServer)) return _metaServer;

                if (Meta != null && Meta.TryGetValue(Env.ToString(), out var meta)
                    && !string.IsNullOrWhiteSpace(meta)) return meta;

                return ConfigConsts.DefaultMetaServerUrl;
            }
            set => _metaServer = ConfigConsts.DefaultMetaServerUrl == value ? null : value;
        }

        public IReadOnlyCollection<string>? ConfigServer { get; set; }

        /// <summary>ms. Default 5000ms</summary>
        public virtual int Timeout { get; set; } = 5000; //5 secondss

        /// <summary>ms. Default 300,000ms</summary>
        public virtual int RefreshInterval { get; set; } = 5 * 60 * 1000; //5 minutes

        public string? LocalCacheDir { get; set; }

        public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Func<HttpMessageHandler>? HttpMessageHandlerFactory { get; set; }

        public ICacheFileProvider CacheFileProvider { get; set; } = new LocalPlaintextCacheFileProvider();
    }
}
