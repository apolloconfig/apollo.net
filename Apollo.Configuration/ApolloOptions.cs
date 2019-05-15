using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using System;
using System.IO;
using System.Text;
using Com.Ctrip.Framework.Apollo.Foundation;
using System.Net.Http;

namespace Com.Ctrip.Framework.Apollo
{
    public class ApolloOptions : IApolloOptions
    {
        /// <summary>
        /// Get the app id for the current application.
        /// </summary>
        /// <returns> the app id or ConfigConsts.NO_APPID_PLACEHOLDER if app id is not available</returns>
        private string _appId;

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
        public virtual string Cluster { get; set; }

        /// <summary>Default Dev</summary>
        public virtual Env Env { get; set; } = Env.Dev;

        public string SubEnv { get; set; }

        public virtual string LocalIp { get; set; } = NetworkInterfaceManager.HostIp;

        /// <summary>Default http://localhost:8080</summary>
        public virtual string MetaServer { get; set; } = ConfigConsts.DefaultMetaServerUrl;

        /// <summary>ms. Default 5000ms</summary>
        public virtual int Timeout { get; set; } = 5000; //5 secondss

        /// <summary>Default "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:")</summary>
        public string Authorization { get; } = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:"));

        /// <summary>ms. Default 300,000ms</summary>
        public virtual int RefreshInterval { get; set; } = 5 * 60 * 1000; //5 minutes

        public string LocalCacheDir { get; set; }

        public Func<HttpMessageHandler> HttpMessageHandlerFactory { get; set; }

        protected internal void InitCluster()
        {
            //LPT and DEV will be treated as a cluster(lower case)
            if (string.IsNullOrWhiteSpace(Cluster) && (Env.Dev == Env || Env.Lpt == Env))
                Cluster = Env.ToString().ToLower();

            //Use data center as cluster
            if (string.IsNullOrWhiteSpace(Cluster))
                Cluster = DataCenter;

            //Use sub env as cluster
            if (string.IsNullOrWhiteSpace(Cluster))
                Cluster = SubEnv;

            //Use default cluster
            if (string.IsNullOrWhiteSpace(Cluster))
                Cluster = ConfigConsts.ClusterNameDefault;
        }
    }
}
