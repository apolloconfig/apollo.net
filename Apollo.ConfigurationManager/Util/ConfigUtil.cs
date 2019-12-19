using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Foundation;
using Com.Ctrip.Framework.Apollo.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static NameValueCollection? AppSettings { get; set; }
        private static Func<HttpMessageHandler>? _httpMessageHandlerFactory;
        private static ICacheFileProvider? _cacheFileProvider;
        private static readonly Func<Action<LogLevel, string, Exception?>> Logger = () => LogManager.CreateLogger(typeof(ConfigUtil));

        private int _refreshInterval = 5 * 60 * 1000; //5 minutes
        private int _timeout = 5000; //5 seconds, c# has no connectTimeout but response timeout

        public ConfigUtil()
        {
            if (AppSettings == null) AppSettings = ConfigurationManager.AppSettings;

            InitRefreshInterval();
            InitTimeout();
            InitCluster();
        }

        /// <summary>
        /// Get the config from app config via key
        /// </summary>
        /// <returns> the value or null if not found </returns>
        public static string? GetAppConfig(string key)
        {
            var key1 = "Apollo." + key;
            var key2 = "Apollo:" + key;

            var appSettings = AppSettings ?? ConfigurationManager.AppSettings;

            var value = appSettings[key1];
            if (string.IsNullOrEmpty(value))
                value = appSettings[key2];

            if (string.IsNullOrEmpty(value))
                value = Environment.GetEnvironmentVariable(key1);

            if (string.IsNullOrEmpty(value))
                value = Environment.GetEnvironmentVariable(key2);

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
                    Logger().Warn("Apollo.AppId is not set, apollo will only load public namespace configurations!");
                }

                return appId!;
            }
        }

        /// <summary>
        /// Get the data center info for the current application.
        /// </summary>
        /// <returns> the current data center, null if there is no such info. </returns>
        public string? DataCenter => GetAppConfig("DataCenter");

        private void InitCluster()
        {
            //Load data center from app.config
            var cluster = GetAppConfig("Cluster");

            //Use data center as cluster
            if (string.IsNullOrWhiteSpace(cluster))
                cluster = DataCenter;

            //Use default cluster
            if (string.IsNullOrWhiteSpace(cluster))
                cluster = ConfigConsts.ClusterNameDefault;

            Cluster = cluster!;
        }

        /// <summary>
        /// Get the cluster name for the current application.
        /// </summary>
        /// <returns> the cluster name, or "default" if not specified </returns>
        public string Cluster { get; private set; } = ConfigConsts.ClusterNameDefault;

        /// <summary>
        /// Get the current environment.
        /// </summary>
        /// <returns> the env </returns>
        public Env Env => Enum.TryParse(GetAppConfig("Env"), true, out Env env) ? env : Env.Dev;

        public string LocalIp { get; set; } = NetworkInterfaceManager.HostIp;

        public string MetaServer => GetAppConfig("MetaServer") ?? MetaDomainConsts.GetDomain(Env);
#if NET40
        public ReadOnlyCollection<string>? ConfigServer
        {
            get
            {
                var servers = GetAppConfig("ConfigServer")?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                return servers == null ? null : new ReadOnlyCollection<string>(servers);
            }
        }
#else
        public IReadOnlyCollection<string>? ConfigServer => GetAppConfig("ConfigServer")?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
#endif
        private void InitTimeout()
        {
            var timeout = GetAppConfig("Timeout");

            if (string.IsNullOrWhiteSpace(timeout) || int.TryParse(timeout, out _timeout)) return;

            _timeout = 5000;

            Logger().Error($"Config for Apollo.Timeout is invalid: {timeout}");
        }

        public int Timeout => _timeout;

        private static readonly string DefaultAuthorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:"));
        public string Authorization => GetAppConfig("Authorization") ?? DefaultAuthorization;

        private void InitRefreshInterval()
        {
            var refreshInterval = GetAppConfig("RefreshInterval");

            if (string.IsNullOrWhiteSpace(refreshInterval) || int.TryParse(GetAppConfig("RefreshInterval"), out _refreshInterval)) return;

            _refreshInterval = 5 * 60 * 1000;

            Logger().Error($"Config for Apollo.RefreshInterval is invalid: {refreshInterval}");
        }

        public int RefreshInterval => _refreshInterval;

        public string LocalCacheDir => GetAppConfig("LocalCacheDir") ?? Path.Combine(ConfigConsts.DefaultLocalCacheDir, AppId);

        public Func<HttpMessageHandler>? HttpMessageHandlerFactory => _httpMessageHandlerFactory;

        public ICacheFileProvider CacheFileProvider => _cacheFileProvider ??= new LocalCacheFileProvider();

        public static void UseHttpMessageHandlerFactory(Func<HttpMessageHandler> factory) => Interlocked.CompareExchange(ref _httpMessageHandlerFactory, factory, null);

        public static void UseCacheFileProvider(ICacheFileProvider cacheFileProvider) => Interlocked.CompareExchange(ref _cacheFileProvider, cacheFileProvider, null);
    }
}
