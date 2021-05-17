﻿using Com.Ctrip.Framework.Apollo.Core.Dto;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Util.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class RemoteConfigRepository : AbstractConfigRepository
    {
        private static readonly Func<Action<LogLevel, string, Exception?>> Logger = () => LogManager.CreateLogger(typeof(RemoteConfigRepository));
        private static readonly TaskFactory ExecutorService = new(new LimitedConcurrencyLevelTaskScheduler(5));

        private readonly ConfigServiceLocator _serviceLocator;
        private readonly HttpUtil _httpUtil;
        private readonly IApolloOptions _options;
        private readonly RemoteConfigLongPollService _remoteConfigLongPollService;

        private volatile ApolloConfig? _configCache;
        private volatile ServiceDto? _longPollServiceDto;
        private volatile ApolloNotificationMessages? _remoteMessages;
        private ExceptionDispatchInfo? _syncException;
        private readonly Timer _timer;

        public RemoteConfigRepository(string @namespace,
            IApolloOptions configUtil,
            HttpUtil httpUtil,
            ConfigServiceLocator serviceLocator,
            RemoteConfigLongPollService remoteConfigLongPollService) : base(@namespace)
        {
            _options = configUtil;
            _httpUtil = httpUtil;
            _serviceLocator = serviceLocator;
            _remoteConfigLongPollService = remoteConfigLongPollService;

            _timer = new Timer(SchedulePeriodicRefresh);
        }

        public override async Task Initialize()
        {
            await SchedulePeriodicRefresh(true).ConfigureAwait(false);

            _timer.Change(_options.RefreshInterval, _options.RefreshInterval);

            _remoteConfigLongPollService.Submit(Namespace, this);
        }

        public override Properties GetConfig()
        {
            _syncException?.Throw();

            return TransformApolloConfigToProperties(_configCache);
        }

        private async void SchedulePeriodicRefresh(object _) => await SchedulePeriodicRefresh(false).ConfigureAwait(false);

        private async Task SchedulePeriodicRefresh(bool isFirst)
        {
            try
            {
                Logger().Debug($"refresh config for namespace: {Namespace}");

                await Sync(isFirst).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _syncException = ExceptionDispatchInfo.Capture(ex);

                Logger().Warn($"refresh config error for namespace: {Namespace}", ex);
            }
        }

        private async Task Sync(bool isFirst)
        {
            var previous = _configCache;
            var current = await LoadApolloConfig(isFirst).ConfigureAwait(false);

            //reference equals means HTTP 304
            if (!ReferenceEquals(previous, current))
            {
                Logger().Debug("Remote Config refreshed!");
                _configCache = current;
                _syncException = null;
                FireRepositoryChange(Namespace, GetConfig());
            }
        }

        private async Task<ApolloConfig?> LoadApolloConfig(bool isFirst)
        {
            var appId = _options.AppId;
            var cluster = _options.Cluster;
            var dataCenter = _options.DataCenter;

            var configServices = await _serviceLocator.GetConfigServices().ConfigureAwait(false);

            Exception? exception = null;
            Uri? url = null;

            var notFound = false;
            for (var i = 0; i < (isFirst ? 1 : 2); i++)
            {
                IList<ServiceDto> randomConfigServices = configServices.OrderBy(_ => Guid.NewGuid()).ToList();

                //Access the server which notifies the client first
                var longPollServiceDto = Interlocked.Exchange(ref _longPollServiceDto, null);
                if (longPollServiceDto != null)
                {
                    randomConfigServices.Insert(0, longPollServiceDto);
                }

                foreach (var configService in randomConfigServices)
                {
                    url = AssembleQueryConfigUrl(configService.HomepageUrl, appId, cluster, Namespace, dataCenter, _remoteMessages!, _configCache!);

                    Logger().Debug($"Loading config from {url}");

                    try
                    {
                        var response = await _httpUtil.DoGetAsync<ApolloConfig?>(url).ConfigureAwait(false);

                        if (response.StatusCode == HttpStatusCode.NotModified)
                        {
                            Logger().Debug("Config server responds with 304 HTTP status code.");
                            return _configCache!;
                        }

                        var result = response.Body;

                        Logger().Debug($"Loaded config for {Namespace}: {result?.Configurations?.Count ?? 0}");

                        return result;
                    }
                    catch (ApolloConfigStatusCodeException ex)
                    {
                        var statusCodeException = ex;
                        //config not found
                        if (ex.StatusCode == HttpStatusCode.NotFound)
                        {
                            notFound = true;

                            var message = $"Could not find config for namespace - appId: {appId}, cluster: {cluster}, namespace: {Namespace}, please check whether the configs are released in Apollo!";
                            statusCodeException = new ApolloConfigStatusCodeException(ex.StatusCode, message);
                        }

                        Logger().Warn(statusCodeException);
                        exception = statusCodeException;
                    }
                    catch (Exception ex)
                    {
                        Logger().Warn(ex);
                        exception = ex;
                    }
                }
#if NET40
                await TaskEx.Delay(1000).ConfigureAwait(false);
#else
                await Task.Delay(1000).ConfigureAwait(false);
#endif
            }

            if (notFound)
                return null;

            var fallbackMessage = $"Load Apollo Config failed - appId: {appId}, cluster: {cluster}, namespace: {Namespace}, url: {url}";

            throw new ApolloConfigException(fallbackMessage, exception!);
        }

        private Uri AssembleQueryConfigUrl(string uri,
            string appId,
            string cluster,
            string? namespaceName,
            string? dataCenter,
            ApolloNotificationMessages? remoteMessages,
            ApolloConfig? previousConfig)
        {
            if (!uri.EndsWith("/", StringComparison.Ordinal))
            {
                uri += "/";
            }
            //Looks like .Net will handle all the url encoding for me...
            var path = $"configs/{appId}/{cluster}/{namespaceName}";
            var uriBuilder = new UriBuilder(uri + path);
            var query = new Dictionary<string, string>();

            if (previousConfig != null)
            {
                query["releaseKey"] = previousConfig.ReleaseKey;
            }

            if (!string.IsNullOrEmpty(dataCenter))
            {
                query["dataCenter"] = dataCenter!;
            }

            var localIp = _options.LocalIp;
            if (!string.IsNullOrEmpty(localIp))
            {
                query["ip"] = localIp;
            }

            if (remoteMessages != null)
            {
                query["messages"] = JsonConvert.SerializeObject(remoteMessages, JsonSettings);
            }

            uriBuilder.Query = QueryUtils.Build(query);

            return uriBuilder.Uri;
        }

        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static Properties TransformApolloConfigToProperties(ApolloConfig? apolloConfig) =>
            apolloConfig?.Configurations == null ? new Properties() : new Properties(apolloConfig.Configurations);

        public void OnLongPollNotified(ServiceDto longPollNotifiedServiceDto, ApolloNotificationMessages remoteMessages)
        {
            _longPollServiceDto = longPollNotifiedServiceDto;
            _remoteMessages = remoteMessages;

            ExecutorService.StartNew(async () =>
            {
                try
                {
                    await Sync(false).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger().Warn($"Sync config failed, will retry. Repository {GetType()}, reason: {ex.GetDetailMessage()}");
                }
            });
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _timer.Dispose();
            }

            //释放非托管资源

            _disposed = true;
        }
    }
}
#if NET40
namespace System.Runtime.ExceptionServices
{
    using Reflection;

    internal sealed class ExceptionDispatchInfo
    {
        private readonly object _source;
        private readonly string _stackTrace;

        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo RemoteStackTrace = typeof(Exception).GetField("_remoteStackTraceString", PrivateInstance)!;
        private static readonly FieldInfo Source = typeof(Exception).GetField("_source", PrivateInstance)!;
        private static readonly MethodInfo InternalPreserveStackTrace = typeof(Exception).GetMethod("InternalPreserveStackTrace", PrivateInstance)!;

        private ExceptionDispatchInfo(Exception source)
        {
            SourceException = source;
            _stackTrace = SourceException.StackTrace + Environment.NewLine;
            _source = Source.GetValue(SourceException);
        }

        public Exception SourceException { get; }

        public static ExceptionDispatchInfo Capture(Exception source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return new ExceptionDispatchInfo(source);
        }

        public void Throw()
        {
            try
            {
                throw SourceException;
            }
            catch
            {
                InternalPreserveStackTrace.Invoke(SourceException, new object[0]);
                RemoteStackTrace.SetValue(SourceException, _stackTrace);
                Source.SetValue(SourceException, _source);
                throw;
            }
        }
    }
}
#endif
