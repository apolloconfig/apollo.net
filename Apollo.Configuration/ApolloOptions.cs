using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using System;
using System.IO;
using Com.Ctrip.Framework.Foundation.Internals;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloOptions: IApolloOptions
    {
        /// <summary>
        /// Get the app id for the current application.
        /// </summary>
        /// <returns> the app id or ConfigConsts.NO_APPID_PLACEHOLDER if app id is not available</returns>
        private string _appId;

        private string _cluster;

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
        public virtual string DataCenter { get; set; }

        /// <summary>
        /// Get the cluster name for the current application.
        /// </summary>
        /// <returns> the cluster name, or "default" if not specified </returns>
        public virtual string Cluster
        {
            get
            {
                if (_cluster == null)
                {
                    //LPT and DEV will be treated as a cluster(lower case)
                    if (string.IsNullOrWhiteSpace(_cluster) && (Env.Dev == ApolloEnv || Env.Lpt == ApolloEnv))
                        _cluster = ApolloEnv.ToString().ToLower();

                    //Use data center as cluster
                    if (string.IsNullOrWhiteSpace(_cluster))
                        _cluster = DataCenter;

                    //Use sub env as cluster
                    if (string.IsNullOrWhiteSpace(_cluster))
                        _cluster = SubEnv;

                    //Use default cluster
                    if (string.IsNullOrWhiteSpace(_cluster))
                        _cluster = ConfigConsts.ClusterNameDefault;
                }

                return _cluster;
            }
            set => _cluster = value;
        }

        /// <summary>
        /// Get the current environment.
        /// </summary>
        /// <returns> the env </returns>
        public virtual Env ApolloEnv { get; set; } = Env.Dev;

        public string SubEnv { get; set; }

        public virtual string LocalIp { get; set; } = NetworkInterfaceManager.HostIp;

        public virtual string MetaServer { get; set; } = ConfigConsts.DefaultMetaServerUrl;

        /// <summary>ms</summary>
        public virtual int Timeout { get; set; } = 5000; //5 secondss

        /// <summary>ms</summary>
        public virtual int ReadTimeout { get; set; } = 5000; //5 seconds

        /// <summary>ms</summary>
        public virtual int RefreshInterval { get; set; } = 5 * 60 * 1000; //5 minutes

        public string LocalCacheDir { get; set; }
    }
}
