using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace Com.Ctrip.Framework.Apollo.Util
{
    public class ConfigUtil : IApolloOptions
    {
        internal static NameValueCollection AppSettings { get; set; }

        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(ConfigUtil));
        private int _refreshInterval = 5 * 60 * 1000; //5 minutes
        private int _timeout = 5000; //5 seconds, c# has no connectTimeout but response timeout
        private int _readTimeout = 5000; //5 seconds
        private string _cluster;

        public ConfigUtil()
        {
            if (AppSettings == null)
                AppSettings = ConfigurationManager.AppSettings;

            InitRefreshInterval();
            InitTimeout();
            InitReadTimeout();
            InitCluster();
        }

        /// <summary>
        /// Get the app id for the current application.
        /// </summary>
        /// <returns> the app id or ConfigConsts.NO_APPID_PLACEHOLDER if app id is not available</returns>
        public string AppId
        {
            get
            {
                var appId = Foundation.Foundation.App.AppId;
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
        public string DataCenter => Foundation.Foundation.Server.DataCenter;

        public string SubEnv => Foundation.Foundation.Server.SubEnvType;

        private void InitCluster()
        {
            //Load data center from app.config
            _cluster = GetAppConfig("Apollo.Cluster");

            var env = Foundation.Foundation.Server.EnvType;
            //LPT and DEV will be treated as a cluster(lower case)
            if (string.IsNullOrWhiteSpace(_cluster) &&
                (Env.Dev.ToString().Equals(env, StringComparison.CurrentCultureIgnoreCase) ||
                 Env.Lpt.ToString().Equals(env, StringComparison.CurrentCultureIgnoreCase))
                )
            {
                _cluster = env.ToLower();
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
        /// <exception cref="ApolloConfigException"> if env is not set </exception>
        public Env ApolloEnv
        {
            get
            {
                var env = EnvUtils.TransformEnv(Foundation.Foundation.Server.EnvType);
                if (env == null)
                {
                    string message = null;
                    if (string.IsNullOrWhiteSpace(Foundation.Foundation.Server.EnvType))
                    {
                        message = $"env is not set, please make sure it is set in {ConfigConsts.ServerPropertiesFile}!";
                    }
                    else
                    {
                        message = $"Env {Foundation.Foundation.Server.EnvType} is unknown to Apollo, please correct it in {ConfigConsts.ServerPropertiesFile}!";
                    }
                    Logger.Error(message);
                    throw new ApolloConfigException(message);
                }
                return (Env)env;
            }
        }

        /// <summary>
        /// Get the config from app config via key
        /// </summary>
        /// <returns> the value or null if not found </returns>
        public string GetAppConfig(string key)
        {
            return AppSettings[key];
        }

        public string LocalIp => Foundation.Foundation.Net.HostAddress;

        public string MetaServer => MetaDomainConsts.GetDomain(ApolloEnv);

        private void InitTimeout()
        {
            var customizedTimeout = GetAppConfig("Apollo.Timeout");
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

        private void InitReadTimeout()
        {
            var customizedReadTimeout = GetAppConfig("Apollo.ReadTimeout");
            if (customizedReadTimeout != null)
            {
                try
                {
                    _readTimeout = int.Parse(customizedReadTimeout);
                }
                catch (Exception)
                {
                    Logger.Error(
                        $"Config for Apollo.ReadTimeout is invalid: {customizedReadTimeout}");
                }
            }
        }

        public int ReadTimeout => _readTimeout;

        private void InitRefreshInterval()
        {
            var customizedRefreshInterval = GetAppConfig("Apollo.RefreshInterval");
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

        public string LocalCacheDir => Path.Combine(ConfigConsts.DefaultLocalCacheDir, AppId);

        public bool InLocalMode
        {
            get
            {
                try
                {
                    var env = ApolloEnv;
                    return Env.Local.Equals(env);
                }
                catch (Exception)
                {
                    //ignore
                }
                return false;
            }
        }

    }
}
