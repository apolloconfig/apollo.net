using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Foundation;
using Com.Ctrip.Framework.Apollo.Logging;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Com.Ctrip.Framework.Apollo.Util
{
    public class ConfigUtil : IApolloOptions
    {
        internal static NameValueCollection AppSettings { get; set; }
        private static Func<HttpMessageHandler> _httpMessageHandlerFactory;

        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(ConfigUtil));
        private int _refreshInterval = 5 * 60 * 1000; //5 minutes
        private int _timeout = 5000; //5 seconds, c# has no connectTimeout but response timeout
        private string _cluster;

        public ConfigUtil()
        {
            if (AppSettings == null)
                AppSettings = ConfigurationManager.AppSettings;

            InitRefreshInterval();
            InitTimeout();
            InitCluster();
        }

        /// <summary>
        /// Get the config from app config via key
        /// </summary>
        /// <returns> the value or null if not found </returns>
        public static string GetAppConfig(string key)
        {
            var value = AppSettings["Apollo." + key];

            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// Get the app id for the current application.
        /// </summary>
        /// <returns> the app id or ConfigConsts.NO_APPID_PLACEHOLDER if app id is not available</returns>
        public string AppId
        {
            get
            {
                var appId = GetAppConfig("AppId");
                if (string.IsNullOrWhiteSpace(appId))
                {
                    appId = ConfigConsts.NoAppidPlaceholder;
                    Logger.Warn("app.id is not set, apollo will only load public namespace configurations!");
                }

                return appId;
            }
        }

        /// <summary>
        /// Get the data center info for the current application.
        /// </summary>
        /// <returns> the current data center, null if there is no such info. </returns>
        public string DataCenter => GetAppConfig("DataCenter");

        public string SubEnv => GetAppConfig("SubEnv");

        private void InitCluster()
        {
            //Load data center from app.config
            _cluster = GetAppConfig("Cluster");

            //LPT and DEV will be treated as a cluster(lower case)
            if (string.IsNullOrWhiteSpace(_cluster) && (Env == Env.Dev || Env == Env.Lpt))
            {
                _cluster = Env.ToString().ToLower();
            }

            //Use data center as cluster
            if (string.IsNullOrWhiteSpace(_cluster))
            {
                _cluster = DataCenter;
            }

            //Use sub env as cluster
            if (string.IsNullOrWhiteSpace(_cluster))
            {
                _cluster = SubEnv;
            }

            //Use default cluster
            if (string.IsNullOrWhiteSpace(_cluster))
            {
                _cluster = ConfigConsts.ClusterNameDefault;
            }
        }

        /// <summary>
        /// Get the cluster name for the current application.
        /// </summary>
        /// <returns> the cluster name, or "default" if not specified </returns>
        public string Cluster => _cluster;

        /// <summary>
        /// Get the current environment.
        /// </summary>
        /// <returns> the env </returns>
        public Env Env => Enum.TryParse(GetAppConfig("Env"), true, out Env env) ? env : Env.Dev;

        public string LocalIp { get; set; } = NetworkInterfaceManager.HostIp;

        public string MetaServer => GetAppConfig("MetaServer") ?? MetaDomainConsts.GetDomain(Env);

        private void InitTimeout()
        {
            var customizedTimeout = GetAppConfig("Timeout");
            if (customizedTimeout != null)
            {
                try
                {
                    _timeout = int.Parse(customizedTimeout);
                }
                catch (Exception)
                {
                    Logger.Error(
                        $"Config for Apollo.Timeout is invalid: {customizedTimeout}");
                }
            }
        }

        public int Timeout => _timeout;

        private static readonly string DefaultAuthorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:"));
        public string Authorization => GetAppConfig("Authorization") ?? DefaultAuthorization;

        private void InitRefreshInterval()
        {
            var customizedRefreshInterval = GetAppConfig("RefreshInterval");

            if (customizedRefreshInterval != null)
            {
                try
                {
                    _refreshInterval = int.Parse(customizedRefreshInterval);
                }
                catch (Exception)
                {
                    Logger.Error(
                        $"Config for Apollo.RefreshInterval is invalid: {customizedRefreshInterval}");
                }
            }
        }

        public int RefreshInterval => _refreshInterval;

        public string LocalCacheDir => GetAppConfig("LocalCacheDir") ?? Path.Combine(ConfigConsts.DefaultLocalCacheDir, AppId);

        public Func<HttpMessageHandler> HttpMessageHandlerFactory => _httpMessageHandlerFactory;

        public static void UseHttpMessageHandlerFactory(Func<HttpMessageHandler> factory) => Interlocked.CompareExchange(ref _httpMessageHandlerFactory, factory, null);
    }
}
