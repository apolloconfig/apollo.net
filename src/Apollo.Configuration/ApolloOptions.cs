using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Foundation;

namespace Com.Ctrip.Framework.Apollo;

public class ApolloOptions : IApolloOptions
{
    /// <summary>
    /// Get the app id for the current application.
    /// </summary>
    /// <returns> the app id or ConfigConsts.NO_APPID_PLACEHOLDER if app id is not available</returns>
    private string _appId = ConfigConsts.NoAppidPlaceholder;
    private string? _dataCenter;
    private string? _cluster;
    private string? _metaServer;

    public string AppId
    {
        get => _appId;
        set
        {
            LocalCacheDir ??= Path.Combine(ConfigConsts.DefaultLocalCacheDir, value);

            _appId = value;
        }
    }

    /// <summary>
    /// Get the data center info for the current application.
    /// </summary>
    /// <returns> the current data center, null if there is no such info. </returns>
    public virtual string? DataCenter
    {
        get => _dataCenter;
        set
        {
            _dataCenter = value;

            if (string.IsNullOrEmpty(_cluster) && !string.IsNullOrEmpty(_dataCenter))
                _cluster = _dataCenter;
        }
    }

    /// <summary>
    /// Get the cluster name for the current application.
    /// </summary>
    /// <returns> the cluster name, or "default" if not specified </returns>
    public virtual string Cluster { get => _cluster ?? ConfigConsts.ClusterNameDefault; set => _cluster = value; }

    public IEnumerable<string>? Namespaces { get; set; }

    /// <summary>Default Dev</summary>
    public virtual Env Env { get; set; } = Env.Dev;

    private string? _localIp;
    private IReadOnlyCollection<string>? _preferSubnet;

    public virtual IReadOnlyCollection<string>? PreferSubnet
    {
        get => _preferSubnet;
        set
        {
            _preferSubnet = value; _localIp = null;
        }
    }

    public virtual string LocalIp
    {
        get => _localIp ??= NetworkInterfaceManager.GetHostIp(_preferSubnet);
        set => _localIp = value;
    }

    /// <summary>Default http://localhost:8080</summary>
    public virtual string? MetaServer
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_metaServer)) return _metaServer;

            if (Meta != default! && Meta.TryGetValue(Env.ToString(), out var meta)
                                 && !string.IsNullOrWhiteSpace(meta)) return meta;

            return ConfigConsts.DefaultMetaServerUrl;
        }
        set => _metaServer = ConfigConsts.DefaultMetaServerUrl == value ? null : value;
    }

    public string? Secret { get; set; }

    public IReadOnlyCollection<string>? ConfigServer { get; set; }

    /// <summary>ms. Default 5000ms</summary>
    public virtual int Timeout { get; set; } = 5000; //5 secondss

    /// <summary>ms. Default 300,000ms</summary>
    public virtual int RefreshInterval { get; set; } = 5 * 60 * 1000; //5 minutes

    public string? LocalCacheDir { get; set; }

    public IDictionary<string, string> Meta { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private HttpMessageHandler _handler = new HttpClientHandler();

    public HttpMessageHandler HttpMessageHandler
    {
        get => _handler;
        set
        {
            if (_handler == value) return;

            Interlocked.Exchange(ref _handler, value).Dispose();
        }
    }

    [Obsolete("Please using the HttpMessageHandler property to configure.", true)]
    public Func<HttpMessageHandler> HttpMessageHandlerFactory
    {
        get => () => _handler;
        set => HttpMessageHandler = value();
    }

    public ICacheFileProvider CacheFileProvider { get; set; } = new LocalPlaintextCacheFileProvider();

    /// <inheritdoc />
    public int StartupTimeout { get; set; } = 30000;

    public IReadOnlyCollection<string>? SpecialDelimiter { get; set; }

    public void Dispose() => _handler.Dispose();
}
