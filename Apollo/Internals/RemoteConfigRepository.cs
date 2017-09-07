using Com.Ctrip.Framework.Apollo.Amib.Threading;
using Com.Ctrip.Framework.Apollo.Core.Dto;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Util.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class RemoteConfigRepository : AbstractConfigRepository
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RemoteConfigRepository));
        private ConfigServiceLocator m_serviceLocator;
        private HttpUtil m_httpUtil;
        private ConfigUtil m_configUtil;
        private volatile ThreadSafe.AtomicReference<ApolloConfig> m_configCache;
        private string m_namespace;
        private static SmartThreadPool m_executorService;
        private readonly RemoteConfigLongPollService m_remoteConfigLongPollService;
        private ThreadSafe.AtomicReference<ServiceDTO> m_longPollServiceDto;
        private ThreadSafe.AtomicReference<ApolloNotificationMessages> m_remoteMessages;

        static RemoteConfigRepository()
        {
            m_executorService = ThreadPoolUtil.NewThreadPool(1, 5, SmartThreadPool.DefaultIdleTimeout, true);
        }

        public RemoteConfigRepository(string namespaceName)
        {
            m_namespace = namespaceName;
            m_configCache = new ThreadSafe.AtomicReference<ApolloConfig>(null);
            m_configUtil = ComponentLocator.Lookup<ConfigUtil>();
            m_httpUtil = ComponentLocator.Lookup<HttpUtil>();
            m_serviceLocator = ComponentLocator.Lookup<ConfigServiceLocator>();
            m_remoteConfigLongPollService = ComponentLocator.Lookup<RemoteConfigLongPollService>();
            m_longPollServiceDto = new ThreadSafe.AtomicReference<ServiceDTO>(null);
            m_remoteMessages = new ThreadSafe.AtomicReference<ApolloNotificationMessages>(null);
            this.TrySync();
            this.SchedulePeriodicRefresh();
            this.ScheduleLongPollingRefresh();
        }

        public override Properties GetConfig()
        {
            if (m_configCache.ReadFullFence() == null)
            {
                this.Sync();
            }
            return TransformApolloConfigToProperties(m_configCache.ReadFullFence());
        }

        public override void SetUpstreamRepository(ConfigRepository upstreamConfigRepository)
        {
            //remote config doesn't need upstream
        }

        private void SchedulePeriodicRefresh()
        {
            logger.Debug(
                string.Format("Schedule periodic refresh with interval: {0} ms",
                m_configUtil.RefreshInterval));

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(m_configUtil.RefreshInterval);
                        logger.Debug(string.Format("refresh config for namespace: {0}", m_namespace));
                        TrySync();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        protected override void Sync()
        {
            lock (this)
            {
                try
                {
                    ApolloConfig previous = m_configCache.ReadFullFence();
                    ApolloConfig current = LoadApolloConfig();
                    
                    //reference equals means HTTP 304
                    if (!object.ReferenceEquals(previous, current))
                    {
                        logger.Debug("Remote Config refreshed!");
                        m_configCache.WriteFullFence(current);
                        this.FireRepositoryChange(m_namespace, this.GetConfig());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private ApolloConfig LoadApolloConfig()
        {
            string appId = m_configUtil.AppId;
            string cluster = m_configUtil.Cluster;
            string dataCenter = m_configUtil.DataCenter;
            int maxRetries = 2;
            Exception exception = null;

            IList<ServiceDTO> configServices = ConfigServices;
            string url = null;
            for (int i = 0; i < maxRetries; i++)
            {
                IList<ServiceDTO> randomConfigServices = new List<ServiceDTO>(configServices);
                randomConfigServices.Shuffle();
                //Access the server which notifies the client first
                if (m_longPollServiceDto.ReadFullFence() != null)
                {
                    randomConfigServices.Insert(0, m_longPollServiceDto.AtomicExchange(null));
                }

                foreach (ServiceDTO configService in randomConfigServices)
                {
                    url = AssembleQueryConfigUrl(configService.HomepageUrl, appId, cluster, m_namespace, dataCenter, m_remoteMessages.ReadFullFence(), m_configCache.ReadFullFence());

                    logger.Debug(string.Format("Loading config from {0}", url));
                    Com.Ctrip.Framework.Apollo.Util.Http.HttpRequest request = new Com.Ctrip.Framework.Apollo.Util.Http.HttpRequest(url);

                    try
                    {
                        HttpResponse<ApolloConfig> response = m_httpUtil.DoGet<ApolloConfig>(request);

                        if (response.StatusCode == 304)
                        {
                            logger.Debug("Config server responds with 304 HTTP status code.");
                            return m_configCache.ReadFullFence();
                        }

                        ApolloConfig result = response.Body;

                        logger.Debug(
                            string.Format("Loaded config for {0}: {1}", m_namespace, result));

                        return result;
                    }
                    catch (ApolloConfigStatusCodeException ex)
                    {
                        ApolloConfigStatusCodeException statusCodeException = ex;
                        //config not found
                        if (ex.StatusCode == 404)
                        {
                            string message = string.Format("Could not find config for namespace - appId: {0}, cluster: {1}, namespace: {2}, please check whether the configs are released in Apollo!", appId, cluster, m_namespace);
                            statusCodeException = new ApolloConfigStatusCodeException(ex.StatusCode, message);
                        }
                        logger.Warn(statusCodeException);
                        exception = statusCodeException;
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex);
                        exception = ex;
                    }

                }

                Thread.Sleep(1000); //sleep 1 second
            }
            string fallbackMessage = string.Format("Load Apollo Config failed - appId: {0}, cluster: {1}, namespace: {2}, url: {3}", appId, cluster, m_namespace, url);
            throw new ApolloConfigException(fallbackMessage, exception);
        }

        private string AssembleQueryConfigUrl(string uri, string appId, string cluster, string namespaceName,
                                              string dataCenter, ApolloNotificationMessages remoteMessages, ApolloConfig previousConfig)
        {
            if (!uri.EndsWith("/", StringComparison.Ordinal))
            {
                uri += "/";
            }
            //Looks like .Net will handle all the url encoding for me...
            string path = string.Format("configs/{0}/{1}/{2}",appId, cluster, namespaceName);
            UriBuilder uriBuilder = new UriBuilder(uri + path);
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (previousConfig != null)
            {
                query["releaseKey"] = previousConfig.ReleaseKey;
            }

            if (!string.IsNullOrEmpty(dataCenter))
            {
                query["dataCenter"] = dataCenter;
            }

            string localIp = m_configUtil.LocalIp;
            if (!string.IsNullOrEmpty(localIp))
            {
                query["ip"] = localIp;
            }

            if (remoteMessages != null)
            {
                query["messages"] = JSON.SerializeObject(remoteMessages);
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }

        private Properties TransformApolloConfigToProperties(ApolloConfig apolloConfig)
        {
            return new Properties(apolloConfig.Configurations);
        }

        private void ScheduleLongPollingRefresh()
        {
            m_remoteConfigLongPollService.Submit(m_namespace, this);
        }

        public void OnLongPollNotified(ServiceDTO longPollNotifiedServiceDto, ApolloNotificationMessages remoteMessages)
        {
            m_longPollServiceDto.WriteFullFence(longPollNotifiedServiceDto);
            m_remoteMessages.WriteFullFence(remoteMessages);
            m_executorService.QueueWorkItem(() =>
            {
                TrySync();
            });
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
