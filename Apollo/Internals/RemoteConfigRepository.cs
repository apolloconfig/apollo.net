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
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class RemoteConfigRepository : AbstractConfigRepository
    {
        private static readonly Func<Action<LogLevel, string, Exception>> Logger = () => LogManager.CreateLogger(typeof(RemoteConfigRepository));
        private static readonly TaskFactory ExecutorService = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(5));

        private readonly ConfigServiceLocator _serviceLocator;
        private readonly HttpUtil _httpUtil;
        private readonly IApolloOptions _options;
        private readonly RemoteConfigLongPollService _remoteConfigLongPollService;

        private volatile ApolloConfig _configCache;
        private volatile ServiceDto _longPollServiceDto;
        private volatile ApolloNotificationMessages _remoteMessages;
        private ExceptionDispatchInfo _syncException;
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

                _syncException = null;
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
                FireRepositoryChange(Namespace, GetConfig());
            }
        }

        private async Task<ApolloConfig> LoadApolloConfig(bool isFirst)
        {
            var appId = _options.AppId;
            var cluster = _options.Cluster;
            var dataCenter = _options.DataCenter;

            var configServices = await _serviceLocator.GetConfigServices().ConfigureAwait(false);

            Exception exception = null;
            string url = null;

            var notFound = false;
            for (var i = 0; i < (isFirst ? 1 : 2); i++)
            {
                IList<ServiceDto> randomConfigServices = new List<ServiceDto>(configServices);
                randomConfigServices.Shuffle();

                //Access the server which notifies the client first
                var longPollServiceDto = Interlocked.Exchange(ref _longPollServiceDto, null);
                if (longPollServiceDto != null)
                {
                    randomConfigServices.Insert(0, longPollServiceDto);
                }

                foreach (var configService in randomConfigServices)
                {
                    url = AssembleQueryConfigUrl(configService.HomepageUrl, appId, cluster, Namespace, dataCenter, _remoteMessages, _configCache);

                    Logger().Debug($"Loading config from {url}");

                    try
                    {
                        var response = await _httpUtil.DoGetAsync<ApolloConfig>(url).ConfigureAwait(false);

                        if (response.StatusCode == HttpStatusCode.NotModified)
                        {
                            Logger().Debug("Config server responds with 304 HTTP status code.");
                            return _configCache;
                        }

                        var result = response.Body;

                        Logger().Debug(
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

                        Logger().Warn(statusCodeException);
                        exception = statusCodeException;
                    }
                    catch (Exception ex)
                    {
                        Logger().Warn(ex);
                        exception = ex;
                    }
                }

                await Task.Delay(1000).ConfigureAwait(false); //sleep 1 second
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
            var query = HttpUtility.ParseQueryString("");

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

            uriBuilder.Query = query.ToString();

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

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _remoteConfigLongPollService.Dispose();
            }

            //释放非托管资源

            _disposed = true;
        }
    }
}
