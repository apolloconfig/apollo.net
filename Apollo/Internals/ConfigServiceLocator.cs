using Com.Ctrip.Framework.Apollo.Core.Dto;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Util.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class ConfigServiceLocator : IDisposable
    {
        private static readonly Action<LogLevel, string, Exception> Logger = LogManager.CreateLogger(typeof(ConfigServiceLocator));

        private readonly HttpUtil _httpUtil;

        private readonly IApolloOptions _options;
        private readonly ThreadSafe.AtomicReference<IList<ServiceDto>> _configServices;
        private Task _updateConfigServicesTask;
        private readonly Timer _timer;

        public ConfigServiceLocator(HttpUtil httpUtil, IApolloOptions configUtil)
        {
            _httpUtil = httpUtil;
            _options = configUtil;
            _configServices = new ThreadSafe.AtomicReference<IList<ServiceDto>>(new List<ServiceDto>());

            _timer = new Timer(SchedulePeriodicRefresh, null, 0, _options.RefreshInterval);
        }

        /// <summary>
        /// Get the config service info from remote meta server.
        /// </summary>
        /// <returns> the services dto </returns>
        public async Task<IList<ServiceDto>> GetConfigServices()
        {
            var services = _configServices.ReadFullFence();
            if (services.Count == 0)
                await UpdateConfigServices().ConfigureAwait(false);

            services = _configServices.ReadFullFence();
            if (services.Count == 0)
                throw new ApolloConfigException("No available config service");

            return services;
        }

        private async void SchedulePeriodicRefresh(object _)
        {
            try
            {
                Logger.Debug("refresh config services");

                await UpdateConfigServices().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }
        }

        private Task UpdateConfigServices()
        {
            Task task;
            if ((task = _updateConfigServicesTask) == null)
                lock (this)
                    if ((task = _updateConfigServicesTask) == null)
                        task = _updateConfigServicesTask = UpdateConfigServices(3);

            return task;
        }

        private async Task UpdateConfigServices(int times)
        {
            var url = AssembleMetaServiceUrl();

            Exception exception = null;

            for (var i = 0; i < times; i++)
            {
                try
                {
                    var response = await _httpUtil.DoGetAsync<IList<ServiceDto>>(url, 2000).ConfigureAwait(false);
                    var services = response.Body;
                    if (services == null || services.Count == 0)
                    {
                        continue;
                    }

                    _configServices.WriteFullFence(services);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                    exception = ex;
                }
            }

            throw new ApolloConfigException($"Get config services failed from {url}", exception);
        }

        private string AssembleMetaServiceUrl()
        {
            var domainName = _options.MetaServer;
            var appId = _options.AppId;
            var localIp = _options.LocalIp;

            var uriBuilder = new UriBuilder(domainName + "/services/config");
            var query = new Dictionary<string, string>();

            query["appId"] = appId;
            if (!string.IsNullOrEmpty(localIp))
            {
                query["ip"] = localIp;
            }

            uriBuilder.Query = QueryUtils.Build(query);

            return uriBuilder.ToString();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
