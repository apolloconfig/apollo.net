using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Dto;
using Com.Ctrip.Framework.Apollo.Core.Schedule;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Util.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Com.Ctrip.Framework.Apollo.Internals;

public class RemoteConfigLongPollService : IDisposable
{
    private static readonly Func<Action<LogLevel, string, Exception?>> Logger = () => LogManager.CreateLogger(typeof(RemoteConfigLongPollService));
    private const long InitNotificationId = -1;
    private readonly ConfigServiceLocator _serviceLocator;
    private readonly HttpUtil _httpUtil;
    private readonly IApolloOptions _options;
    private CancellationTokenSource? _cts;
    private readonly ISchedulePolicy _longPollFailSchedulePolicyInSecond;
    private readonly ISchedulePolicy _longPollSuccessSchedulePolicyInMs;
    private readonly ConcurrentDictionary<string, ISet<RemoteConfigRepository>> _longPollNamespaces;
    private readonly ConcurrentDictionary<string, long?> _notifications;
    private readonly ConcurrentDictionary<string, ApolloNotificationMessages> _remoteNotificationMessages; //namespaceName -> watchedKey -> notificationId

    public RemoteConfigLongPollService(ConfigServiceLocator serviceLocator, HttpUtil httpUtil, IApolloOptions configUtil)
    {
        _serviceLocator = serviceLocator;
        _httpUtil = httpUtil;
        _options = configUtil;
        _longPollFailSchedulePolicyInSecond = new ExponentialSchedulePolicy(1, 120); //in second
        _longPollSuccessSchedulePolicyInMs = new ExponentialSchedulePolicy(100, 1000); //in millisecond
        _longPollNamespaces = new ConcurrentDictionary<string, ISet<RemoteConfigRepository>>();
        _notifications = new ConcurrentDictionary<string, long?>();
        _remoteNotificationMessages = new ConcurrentDictionary<string, ApolloNotificationMessages>();
    }

    public void Submit(string namespaceName, RemoteConfigRepository remoteConfigRepository)
    {
        var remoteConfigRepositories = _longPollNamespaces.GetOrAdd(namespaceName, _ => new HashSet<RemoteConfigRepository>());

        lock (remoteConfigRepositories) remoteConfigRepositories.Add(remoteConfigRepository);

        _notifications.TryAdd(namespaceName, InitNotificationId);

        if (_cts == null) StartLongPolling();
    }

    private void StartLongPolling()
    {
        if (Interlocked.CompareExchange(ref _cts, new CancellationTokenSource(), null) != null) return;

        try
        {
            var unused = DoLongPollingRefresh(_options.AppId, _options.Cluster, _options.DataCenter, _cts.Token);
        }
        catch (Exception ex)
        {
            var exception = new ApolloConfigException("Schedule long polling refresh failed", ex);
            Logger().Warn(exception.GetDetailMessage());
        }
    }

    private async Task DoLongPollingRefresh(string appId, string cluster, string? dataCenter, CancellationToken cancellationToken)
    {
        var random = new Random();
        ServiceDto? lastServiceDto = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            var sleepTime = 50; //default 50 ms
            Uri? url = null;
            try
            {
                if (lastServiceDto == null)
                {
                    var configServices = await _serviceLocator.GetConfigServices().ConfigureAwait(false);
                    lastServiceDto = configServices[random.Next(configServices.Count)];
                }

                url = AssembleLongPollRefreshUrl(lastServiceDto.HomepageUrl, appId, cluster, dataCenter);

                Logger().Debug($"Long polling from {url}");
#if NET40
                    var response = await _httpUtil.DoGetAsync<ICollection<ApolloConfigNotification>>(url, 600000).ConfigureAwait(false);
#else
                var response = await _httpUtil.DoGetAsync<IReadOnlyCollection<ApolloConfigNotification>>(url, 600000).ConfigureAwait(false);
#endif
                Logger().Debug($"Long polling response: {response.StatusCode}, url: {url}");
                if (response.StatusCode == HttpStatusCode.OK && response.Body != null)
                {
                    UpdateNotifications(response.Body);
                    UpdateRemoteNotifications(response.Body);
                    Notify(lastServiceDto, response.Body);
                    _longPollSuccessSchedulePolicyInMs.Success();
                }
                else
                {
                    sleepTime = _longPollSuccessSchedulePolicyInMs.Fail();
                }

                //try to load balance
                if (response.StatusCode == HttpStatusCode.NotModified && random.NextDouble() >= 0.5)
                {
                    lastServiceDto = null;
                }

                _longPollFailSchedulePolicyInSecond.Success();
            }
            catch (Exception ex)
            {
                lastServiceDto = null;

                var sleepTimeInSecond = _longPollFailSchedulePolicyInSecond.Fail();
                Logger().Warn($"Long polling failed, will retry in {sleepTimeInSecond} seconds. appId: {appId}, cluster: {cluster}, namespace: {string.Join(ConfigConsts.ClusterNamespaceSeparator, _longPollNamespaces.Keys)}, long polling url: {url}, reason: {ex.GetDetailMessage()}");

                sleepTime = sleepTimeInSecond * 1000;
            }
            finally
            {
#if NET40
                    await TaskEx.Delay(sleepTime, cancellationToken).ConfigureAwait(false);
#else
                await Task.Delay(sleepTime, cancellationToken).ConfigureAwait(false);
#endif
            }
        }
    }
#if NET40
        private void Notify(ServiceDto lastServiceDto, ICollection<ApolloConfigNotification>? notifications)
#else
    private void Notify(ServiceDto lastServiceDto, IReadOnlyCollection<ApolloConfigNotification>? notifications)
#endif
    {
        if (notifications == null || notifications.Count == 0) return;

        foreach (var notification in notifications)
        {
            var namespaceName = notification.NamespaceName;

            //create a new list to avoid ConcurrentModificationException
            var toBeNotified = new List<RemoteConfigRepository>();
            if (_longPollNamespaces.TryGetValue(namespaceName, out var registries) && registries != null)
                toBeNotified.AddRange(registries);

            //since .properties are filtered out by default, so we need to check if there is any listener for it
            if (_longPollNamespaces.TryGetValue($"{namespaceName}.{ConfigFileFormat.Properties.GetString()}", out registries) && registries != null)
                toBeNotified.AddRange(registries);

            if (!_remoteNotificationMessages.TryGetValue(namespaceName, out var originalMessages)) return;
            var remoteMessages = originalMessages.Clone();
            foreach (var remoteConfigRepository in toBeNotified)
            {
                try
                {
                    remoteConfigRepository.OnLongPollNotified(lastServiceDto, remoteMessages);
                }
                catch (Exception ex)
                {
                    Logger().Warn(ex);
                }
            }
        }
    }

    private void UpdateNotifications(IEnumerable<ApolloConfigNotification> deltaNotifications)
    {
        foreach (var notification in deltaNotifications)
        {
            if (string.IsNullOrEmpty(notification.NamespaceName)) continue;

            var namespaceName = notification.NamespaceName;
            if (_notifications.ContainsKey(namespaceName))
            {
                _notifications[namespaceName] = notification.NotificationId;
            }
            //since .properties are filtered out by default, so we need to check if there is notification with .properties suffix
            var namespaceNameWithPropertiesSuffix = $"{namespaceName}.{ConfigFileFormat.Properties.GetString()}";
            if (_notifications.ContainsKey(namespaceNameWithPropertiesSuffix))
            {
                _notifications[namespaceNameWithPropertiesSuffix] = notification.NotificationId;
            }
        }
    }

    private void UpdateRemoteNotifications(IEnumerable<ApolloConfigNotification> deltaNotifications)
    {
        foreach (var notification in deltaNotifications)
        {
            if (string.IsNullOrEmpty(notification.NamespaceName) || notification.Messages == null || notification.Messages.IsEmpty()) continue;

            var localRemoteMessages = _remoteNotificationMessages.GetOrAdd(notification.NamespaceName, _ => new ApolloNotificationMessages());

            localRemoteMessages.MergeFrom(notification.Messages);
        }
    }

    private Uri AssembleLongPollRefreshUrl(string uri, string appId, string cluster, string? dataCenter)
    {
        if (!uri.EndsWith("/", StringComparison.Ordinal)) uri += "/";

        var uriBuilder = new UriBuilder(uri + "notifications/v2");
        var query = new Dictionary<string, string>();

        query["appId"] = appId;
        query["cluster"] = cluster;
        query["notifications"] = AssembleNotifications(_notifications);

        if (!string.IsNullOrEmpty(dataCenter))
        {
            query["dataCenter"] = dataCenter!;
        }
        var localIp = _options.LocalIp;
        if (!string.IsNullOrEmpty(localIp))
        {
            query["ip"] = localIp;
        }

        uriBuilder.Query = QueryUtils.Build(query);

        return uriBuilder.Uri;
    }

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    private static string AssembleNotifications(IDictionary<string, long?> notificationsMap) =>
        JsonConvert.SerializeObject(notificationsMap
            .Select(kvp => new ApolloConfigNotification
            {
                NamespaceName = kvp.Key,
                NotificationId = kvp.Value.GetValueOrDefault(InitNotificationId)
            }), JsonSettings);

    public void Dispose()
    {
        if (_cts == null) return;

        _cts.Cancel();
        _cts.Dispose();
    }
}