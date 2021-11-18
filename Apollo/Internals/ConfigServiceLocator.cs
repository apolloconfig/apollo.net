using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Dto;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Util.Http;

namespace Com.Ctrip.Framework.Apollo.Internals;

public class ConfigServiceLocator : IDisposable
{
    private static readonly char[] MetaServerSeparator = new[] { ',', ';' };
    private static readonly Func<Action<LogLevel, string, Exception?>> Logger = () => LogManager.CreateLogger(typeof(ConfigServiceLocator));

    private readonly HttpUtil _httpUtil;

    private readonly IApolloOptions _options;
    private volatile IList<ServiceDto> _configServices = new List<ServiceDto>();
    private Task? _updateConfigServicesTask;
    private readonly Timer? _timer;

    public ConfigServiceLocator(HttpUtil httpUtil, IApolloOptions configUtil)
    {
        _httpUtil = httpUtil;
        _options = configUtil;

        var serviceDtos = GetCustomizedConfigService(configUtil);

        if (serviceDtos == null || serviceDtos.Count < 1)
            _timer = new Timer(SchedulePeriodicRefresh, null, 0, _options.RefreshInterval);
        else
            _configServices = serviceDtos;
    }

    private static IList<ServiceDto>? GetCustomizedConfigService(IApolloOptions configUtil) =>
        configUtil.ConfigServer?
            .Select(configServiceUrl => new ServiceDto
            {
                HomepageUrl = configServiceUrl.Trim(),
                InstanceId = configServiceUrl.Trim(),
                AppName = ConfigConsts.ConfigService
            })
            .ToArray();

    /// <summary>
    /// Get the config service info from remote meta server.
    /// </summary>
    /// <returns> the services dto </returns>
    public async Task<IList<ServiceDto>> GetConfigServices()
    {
        var services = _configServices;
        if (services.Count == 0)
            await UpdateConfigServices().ConfigureAwait(false);

        services = _configServices;
        if (services.Count == 0)
            throw new ApolloConfigException("No available config service");

        return services;
    }

    private async void SchedulePeriodicRefresh(object _)
    {
        try
        {
            Logger().Debug("refresh config services");

            await UpdateConfigServices().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Logger().Warn(ex);
        }
    }

    private Task UpdateConfigServices()
    {
        Task? task;
        if ((task = _updateConfigServicesTask) != null) return task;

        lock (this)
            if ((task = _updateConfigServicesTask) == null)
            {
                task = _updateConfigServicesTask = UpdateConfigServices(3);

                _updateConfigServicesTask.ContinueWith(_ => _updateConfigServicesTask = null);
            }

        return task;
    }

    private async Task UpdateConfigServices(int times)
    {
        var url = AssembleMetaServiceUrl();

        Exception? exception = null;

        for (var index = 0; index < Math.Max(url.Count, times); index++)
        {
            try
            {
                var response = await _httpUtil.DoGetAsync<IList<ServiceDto>?>(url[index % url.Count]).ConfigureAwait(false);
                var services = response.Body;
                if (services == null || services.Count == 0) continue;

                _configServices = services;

                return;
            }
            catch (Exception ex)
            {
                Logger().Warn("Update config service fail from " + url[index % url.Count], ex);

                exception = ex;
            }
        }

        throw new ApolloConfigException($"Get config services failed from \"{string.Join(", ",url)}\"", exception!);
    }
#if NET40
    private IList<Uri> AssembleMetaServiceUrl() =>
#else
        private IReadOnlyList<Uri> AssembleMetaServiceUrl() =>
#endif
        (_options.MetaServer?
            .Split(MetaServerSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(uri => Uri.TryCreate(uri, UriKind.Absolute, out _) ? uri : default!)
            .Where(uri => uri != default!)
            .DefaultIfEmpty(ConfigConsts.DefaultMetaServerUrl)
            .ToArray() ?? new[] { ConfigConsts.DefaultMetaServerUrl })
        .Select(uri =>
        {
            if (uri[uri.Length - 1] != '/') uri += "/";

            var uriBuilder = new UriBuilder(uri + "services/config");

            var query = new Dictionary<string, string> { ["appId"] = _options.AppId };

            if (!string.IsNullOrEmpty(_options.LocalIp)) query["ip"] = _options.LocalIp;

            uriBuilder.Query = QueryUtils.Build(query);

            return uriBuilder.Uri;
        })
        .OrderBy(_ => Guid.NewGuid())
        .ToArray();

    public void Dispose() => _timer?.Dispose();
}