using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Core.Ioc;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util.Http;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Core.Schedule;
using System.Collections.Concurrent;
using System.Threading;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Core.Dto;
using System.Web;
using Com.Ctrip.Framework.Apollo.Newtonsoft.Json;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    [Named(ServiceType = typeof(RemoteConfigLongPollService))]
    public class RemoteConfigLongPollService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RemoteConfigLongPollService));
        private static readonly long INIT_NOTIFICATION_ID = -1;
        [Inject]
        private ConfigServiceLocator m_serviceLocator;
        [Inject]
        private HttpUtil m_httpUtil;
        [Inject]
        private ConfigUtil m_configUtil;
        private ThreadSafe.Boolean m_longPollingStarted;
        private ThreadSafe.Boolean m_longPollingStopped;
        private SchedulePolicy m_longPollFailSchedulePolicyInSecond;
        private SchedulePolicy m_longPollSuccessSchedulePolicyInMS;
        private readonly IDictionary<string, ISet<RemoteConfigRepository>> m_longPollNamespaces;
        private readonly IDictionary<string, long?> m_notifications;

        public RemoteConfigLongPollService()
        {
            m_longPollFailSchedulePolicyInSecond = new ExponentialSchedulePolicy(1, 120); //in second
            m_longPollSuccessSchedulePolicyInMS = new ExponentialSchedulePolicy(100, 1000); //in millisecond
            m_longPollingStarted = new ThreadSafe.Boolean(false);
            m_longPollingStopped = new ThreadSafe.Boolean(false);
            m_longPollNamespaces = new ConcurrentDictionary<string, ISet<RemoteConfigRepository>>();
            m_notifications = new ConcurrentDictionary<string, long?>();
        }

        public bool Submit(string namespaceName, RemoteConfigRepository remoteConfigRepository)
        {
            ISet<RemoteConfigRepository> remoteConfigRepositories = null;
            m_longPollNamespaces.TryGetValue(namespaceName, out remoteConfigRepositories);
            if (remoteConfigRepositories == null)
            {
                lock (this)
                {
                    m_longPollNamespaces.TryGetValue(namespaceName, out remoteConfigRepositories);
                    if (remoteConfigRepositories == null)
                    {
                        remoteConfigRepositories = new HashSet<RemoteConfigRepository>();
                        m_longPollNamespaces[namespaceName] = remoteConfigRepositories;
                    }
                }
            }
            bool added = remoteConfigRepositories.Add(remoteConfigRepository);
            if(!m_notifications.ContainsKey(namespaceName))
            {
                lock (this)
                {
                    if (!m_notifications.ContainsKey(namespaceName))
                    {
                        m_notifications[namespaceName] = INIT_NOTIFICATION_ID;
                    }
                }
            }
            if (!m_longPollingStarted.ReadFullFence())
            {
                StartLongPolling();
            }
            return added;
        }


        private void StartLongPolling()
        {
            if (!m_longPollingStarted.CompareAndSet(false, true)) {
                //already started
                return;
            }
            try
            {
                string appId = m_configUtil.AppId;
                string cluster = m_configUtil.Cluster;
                string dataCenter = m_configUtil.DataCenter;

                Thread t = new Thread(() =>
                {
                    DoLongPollingRefresh(appId, cluster, dataCenter);
                });
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            {
                ApolloConfigException exception = new ApolloConfigException("Schedule long polling refresh failed", ex);
                logger.Warn(ExceptionUtil.GetDetailMessage(exception));
            }
        }

        private void StopLongPollingRefresh()
        {
            this.m_longPollingStopped.CompareAndSet(false, true);
        }

        private void DoLongPollingRefresh(string appId, string cluster, string dataCenter)
        {
            Random random = new Random();
            ServiceDTO lastServiceDto = null;

            while (!m_longPollingStopped.ReadFullFence())
            {
                int sleepTime = 50; //default 50 ms
                try
                {
                    if (lastServiceDto == null)
                    {
                        IList<ServiceDTO> configServices = ConfigServices;
                        lastServiceDto = configServices[random.Next(configServices.Count)];
                    }

                    string url = AssembleLongPollRefreshUrl(lastServiceDto.HomepageUrl, appId, cluster, dataCenter);

                    logger.Debug(
                        string.Format("Long polling from {0}", url));
                    Com.Ctrip.Framework.Apollo.Util.Http.HttpRequest request = new Com.Ctrip.Framework.Apollo.Util.Http.HttpRequest(url);
                    //longer timeout - 10 minutes
                    request.Timeout = 600000;

                    HttpResponse<IList<ApolloConfigNotification>> response = m_httpUtil.DoGet<IList<ApolloConfigNotification>>(request);

                    logger.Debug(
                        string.Format("Long polling response: {0}, url: {1}", response.StatusCode, url));
                    if (response.StatusCode == 200 && response.Body != null)
                    {
                        UpdateNotifications(response.Body);
                        Notify(lastServiceDto, response.Body);
                        m_longPollSuccessSchedulePolicyInMS.Success();
                    } else 
                    {
                        sleepTime = m_longPollSuccessSchedulePolicyInMS.Fail();
                    }

                    //try to load balance
                    if (response.StatusCode == 304 && random.NextDouble() >= 0.5)
                    {
                        lastServiceDto = null;
                    }

                    m_longPollFailSchedulePolicyInSecond.Success();
                }
                catch (Exception ex)
                {
                    lastServiceDto = null;

                    int sleepTimeInSecond = m_longPollFailSchedulePolicyInSecond.Fail();
                    logger.Warn(
                        string.Format("Long polling failed, will retry in {0} seconds. appId: {1}, cluster: {2}, namespace: {3}, reason: {4}",
                        sleepTimeInSecond, appId, cluster, AssembleNamespaces(), ExceptionUtil.GetDetailMessage(ex)));

                    sleepTime = sleepTimeInSecond * 1000;
                }
                finally
                {
                    Thread.Sleep(sleepTime);
                }
            }
        }

        private void Notify(ServiceDTO lastServiceDto, IList<ApolloConfigNotification> notifications)
        {
            if (notifications == null || notifications.Count == 0)
            {
                return;
            }
            foreach (ApolloConfigNotification notification in notifications)
            {
                string namespaceName = notification.NamespaceName;
                //create a new list to avoid ConcurrentModificationException
                ISet<RemoteConfigRepository> registries = null;
                List<RemoteConfigRepository> toBeNotified = new List<RemoteConfigRepository>();
                m_longPollNamespaces.TryGetValue(namespaceName, out registries);
                if (registries != null && registries.Count > 0)
                {
                    toBeNotified.AddRange(registries);
                }
                //since .properties are filtered out by default, so we need to check if there is any listener for it
                ISet<RemoteConfigRepository> extraRegistries = null;
                m_longPollNamespaces.TryGetValue(string.Format("{0}.{1}", namespaceName, ConfigFileFormat.Properties.GetString()), out extraRegistries);
                if (extraRegistries != null && extraRegistries.Count > 0)
                {
                    toBeNotified.AddRange(extraRegistries);
                }
                foreach (RemoteConfigRepository remoteConfigRepository in toBeNotified)
                {
                    try
                    {
                        remoteConfigRepository.OnLongPollNotified(lastServiceDto);
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex);
                    }
                }
            }
        }

        private void UpdateNotifications(IList<ApolloConfigNotification> deltaNotifications)
        {
            foreach (ApolloConfigNotification notification in deltaNotifications)
            {
                if (string.IsNullOrEmpty(notification.NamespaceName))
                {
                    continue;
                }
                string namespaceName = notification.NamespaceName;
                if (m_notifications.ContainsKey(namespaceName))
                {
                    m_notifications[namespaceName] = notification.NotificationId;
                }
                //since .properties are filtered out by default, so we need to check if there is notification with .properties suffix
                string namespaceNameWithPropertiesSuffix = string.Format("{0}.{1}", namespaceName, ConfigFileFormat.Properties.GetString());
                if (m_notifications.ContainsKey(namespaceNameWithPropertiesSuffix))
                {
                    m_notifications[namespaceNameWithPropertiesSuffix] = notification.NotificationId;
                }
            }
        }

        private string AssembleNamespaces()
        {
            return string.Join(ConfigConsts.CLUSTER_NAMESPACE_SEPARATOR, m_longPollNamespaces.Keys);
        }

        private string AssembleLongPollRefreshUrl(string uri, string appId, string cluster, string dataCenter)
        {
            if (!uri.EndsWith("/", StringComparison.Ordinal))
            {
                uri += "/";
            }
            UriBuilder uriBuilder = new UriBuilder(uri + "notifications/v2");
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["appId"] = appId;
            query["cluster"] = cluster;
            query["notifications"] = AssembleNotifications(m_notifications);

            if (!string.IsNullOrEmpty(dataCenter))
            {
                query["dataCenter"] = dataCenter;
            }
            string localIp = m_configUtil.LocalIp;
            if (!string.IsNullOrEmpty(localIp))
            {
                query["ip"] = localIp;
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

        private String AssembleNotifications(IDictionary<string, long?> notificationsMap)
        {
            IList<ApolloConfigNotification> notifications = new List<ApolloConfigNotification>();
            foreach (KeyValuePair<string, long?> kvp in notificationsMap)
            {
                ApolloConfigNotification notification = new ApolloConfigNotification();
                notification.NamespaceName = kvp.Key;
                notification.NotificationId = kvp.Value.GetValueOrDefault(INIT_NOTIFICATION_ID);
                notifications.Add(notification);
            }
            return JSON.SerializeObject(notifications);
        }

        private IList<ServiceDTO> ConfigServices
        {
            get
            {
                IList<ServiceDTO> services = m_serviceLocator.GetConfigServices();
                if (services.Count == 0)
                {
                    throw new ApolloConfigException("No available config service");
                }

                return services;
            }
        }
        
    }
}
