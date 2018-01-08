using Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Core.Dto;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Util.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class RemoteConfigRepository : AbstractConfigRepository
    {
        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(RemoteConfigRepository));
        private static readonly TaskFactory ExecutorService = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(5));

        private readonly ConfigServiceLocator _serviceLocator;
        private readonly HttpUtil _httpUtil;
        private readonly IApolloOptions _options;
        private readonly RemoteConfigLongPollService _remoteConfigLongPollService;

        private volatile ThreadSafe.AtomicReference<ApolloConfig> _configCache = new ThreadSafe.AtomicReference<ApolloConfig>(null);
        private readonly ThreadSafe.AtomicReference<ServiceDto> _longPollServiceDto = new ThreadSafe.AtomicReference<ServiceDto>(null);
        private readonly ThreadSafe.AtomicReference<ApolloNotificationMessages> _remoteMessages = new ThreadSafe.AtomicReference<ApolloNotificationMessages>(null);

        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);
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

            AsyncHelper.RunSync(BeginSync);
        }

        private async Task BeginSync()
        {
            await SchedulePeriodicRefresh();

            _timer.Change(_options.RefreshInterval, _options.RefreshInterval);

            ScheduleLongPollingRefresh();
        }

        public override Properties GetConfig()
        {
            if (_configCache.ReadFullFence() == null)
                AsyncHelper.RunSync(SchedulePeriodicRefresh);

            return TransformApolloConfigToProperties(_configCache.ReadFullFence());
        }

        private async void SchedulePeriodicRefresh(object _) => await SchedulePeriodicRefresh();

        private async Task SchedulePeriodicRefresh()
        {
            try
            {
                Logger.Debug($"refresh config for namespace: {Namespace}");

                await Sync();
            }
            catch (Exception ex)
            {
                Logger.Warn($"refresh config error for namespace: {Namespace}", ex);
            }
        }

        private async Task Sync()
        {
            try
            {
                var previous = _configCache.ReadFullFence();
                var current = await LoadApolloConfig();

                //reference equals means HTTP 304
                if (!ReferenceEquals(previous, current))
                {
                    Logger.Debug("Remote Config refreshed!");
                    _configCache.WriteFullFence(current);
                    FireRepositoryChange(Namespace, GetConfig());
                }

                if (!_resetEvent.IsSet)
                    _resetEvent.Set();
            }
            catch (Exception e)
            {
                Logger.Warn(e);
            }
        }

        private async Task<ApolloConfig> LoadApolloConfig()
        {
            var appId = _options.AppId;
            var cluster = _options.Cluster;
            var dataCenter = _options.DataCenter;

            var configServices = await _serviceLocator.GetConfigServices();

            Exception exception = null;
            string url = null;

            var notFound = false;
            for (var i = 0; i < 2; i++)
            {
                IList<ServiceDto> randomConfigServices = new List<ServiceDto>(configServices);
                randomConfigServices.Shuffle();
                //Access the server which notifies the client first
                if (_longPollServiceDto.ReadFullFence() != null)
                {
                    randomConfigServices.Insert(0, _longPollServiceDto.AtomicExchange(null));
                }

                foreach (var configService in randomConfigServices)
                {
                    url = AssembleQueryConfigUrl(configService.HomepageUrl, appId, cluster, Namespace, dataCenter, _remoteMessages.ReadFullFence(), _configCache.ReadFullFence());

                    Logger.Debug($"Loading config from {url}");

                    try
                    {
                        var response = await _httpUtil.DoGetAsync<ApolloConfig>(url);

                        if (response.StatusCode == HttpStatusCode.NotModified)
                        {
                            Logger.Debug("Config server responds with 304 HTTP status code.");
                            return _configCache.ReadFullFence();
                        }

                        var result = response.Body;

                        Logger.Debug(
                            $"Loaded config for {Namespace}: {result?.Configurations?.Count ?? 0}");

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

                        Logger.Warn(statusCodeException);
                        exception = statusCodeException;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex);
                        exception = ex;
                    }
                }

                await Task.Delay(1000); //sleep 1 second
            }

            if (notFound)
                return null;

            var fallbackMessage = $"Load Apollo Config failed - appId: {appId}, cluster: {cluster}, namespace: {Namespace}, url: {url}";

            throw new ApolloConfigException(fallbackMessage, exception);
        }

        private string AssembleQueryConfigUrl(string uri,
            string appId,
            string cluster,
            string namespaceName,
            string dataCenter,
            ApolloNotificationMessages remoteMessages,
            ApolloConfig previousConfig)
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
                query["dataCenter"] = dataCenter;
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

            return uriBuilder.ToString();
        }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private Properties TransformApolloConfigToProperties(ApolloConfig apolloConfig)
        {
            return apolloConfig == null ? new Properties() : new Properties(apolloConfig.Configurations);
        }

        private void ScheduleLongPollingRefresh()
        {
            _remoteConfigLongPollService.Submit(Namespace, this);
        }

        public void OnLongPollNotified(ServiceDto longPollNotifiedServiceDto, ApolloNotificationMessages remoteMessages)
        {
            _longPollServiceDto.WriteFullFence(longPollNotifiedServiceDto);
            _remoteMessages.WriteFullFence(remoteMessages);

            ExecutorService.StartNew(async () =>
            {
                try
                {
                    await Sync();
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Sync config failed, will retry. Repository {GetType()}, reason: {ExceptionUtil.GetDetailMessage(ex)}");
                }
            });
        }

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _resetEvent.Dispose();
            }

            //释放非托管资源

            _disposed = true;
        }
    }
}
