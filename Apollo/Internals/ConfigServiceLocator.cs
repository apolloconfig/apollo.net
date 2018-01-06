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
        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(ConfigServiceLocator));

        private readonly HttpUtil _httpUtil;

        private readonly IApolloOptions _options;
        private readonly ThreadSafe.AtomicReference<IList<ServiceDto>> _configServices;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
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
                await UpdateConfigServices();

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

                await UpdateConfigServices();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }
        }

        private async Task UpdateConfigServices()
        {
            await _semaphore.WaitAsync();
            try
            {
                var url = AssembleMetaServiceUrl();

                var request = new HttpRequest(url);
                var maxRetries = 5;
                Exception exception = null;

                for (var i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        var response = await _httpUtil.DoGetAsync<IList<ServiceDto>>(request);
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

                    await Task.Delay(1000); //sleep 1 second
                }

                throw new ApolloConfigException($"Get config services failed from {url}", exception);
            }
            finally
            {
                _semaphore.Release();
            }
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
            _semaphore?.Dispose();
            _timer?.Dispose();
        }
    }
}
