using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;

namespace Com.Ctrip.Framework.Apollo.Internals;

public class LocalFileConfigRepository : AbstractConfigRepository, IRepositoryChangeListener
{
    private static readonly Func<Action<LogLevel, string, Exception?>> Logger = () =>
        LogManager.CreateLogger(typeof(LocalFileConfigRepository));
#if NET40
    private static readonly Task CompletedTask = TaskEx.FromResult(0);
#elif NET45
    private static readonly Task CompletedTask = Task.FromResult(0);
#else
    private static readonly Task CompletedTask = Task.CompletedTask;
#endif
    private const string ConfigDir = "config-cache";

    private string? _baseDir;
    private volatile Properties? _fileProperties;
    private TaskCompletionSource<object?>? _tcs;

    private readonly IApolloOptions _options;
    private readonly IConfigRepository? _upstream;

    public ConfigFileFormat Format { get; } = ConfigFileFormat.Properties;

    public LocalFileConfigRepository(string @namespace, IApolloOptions configUtil, IConfigRepository? upstream = null)
        : base(@namespace)
    {
        _upstream = upstream;
        _options = configUtil;

        var ext = Path.GetExtension(@namespace);
        if (ext is { Length: > 1 } && Enum.TryParse(ext.Substring(1), true, out ConfigFileFormat format))
            Format = format;

        PrepareConfigCacheDir();
    }

    public override async Task Initialize()
    {
        if (_baseDir != null)
            try
            {
                _fileProperties = LoadFromLocalCacheFile(_baseDir, Namespace);
            }
            catch (Exception ex)
            {
                Logger().Warn(ex);
            }

        if (_upstream != null)
        {
            await _upstream.Initialize().ConfigureAwait(false);

            _upstream.AddChangeListener(this);

            //sync with upstream immediately
            await TrySyncFromUpstream().ConfigureAwait(false);
        }
    }

    public override Properties GetConfig()
    {
        var properties = _fileProperties == null ? new() : new Properties(_fileProperties);

        if (Format == ConfigFileFormat.Properties || !ConfigAdapterRegister.TryGetAdapter(Format, out var adapter))
            return properties.SpecialDelimiter(_options.SpecialDelimiter);

        try
        {
            return adapter.GetProperties(properties);
        }
        catch (Exception ex)
        {
            throw new ApolloConfigException($"Config Error! AppId: {_options.AppId}, Namespace: {Namespace}", ex);
        }
    }

    private bool _disposed;

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing) _upstream?.Dispose();

        //释放非托管资源

        _disposed = true;
    }

    private Task TrySyncFromUpstream()
    {
        if (_upstream == null) return CompletedTask;

        try
        {
            var properties = _upstream.GetConfig();

            UpdateFileProperties(properties);
        }
        catch (Exception ex)
        {
            Logger()
                .Warn(
                    $"Sync config from upstream repository {_upstream.GetType()} failed, reason: {ex.GetDetailMessage()}");

            // If the config fails to be obtained at startup and there is no local cache, wait until successful or timeout.
            if (_fileProperties == null)
#if NET40
                return TaskEx.WhenAny((_tcs = new()).Task, TaskEx.Delay(_options.StartupTimeout));
#else
                return Task.WhenAny((_tcs = new()).Task, Task.Delay(_options.StartupTimeout));
#endif
        }

        return CompletedTask;
    }

    public void OnRepositoryChange(string namespaceName, Properties newProperties)
    {
        Interlocked.Exchange(ref _tcs, null)?.TrySetResult(null);

        UpdateFileProperties(new(newProperties));

        FireRepositoryChange(namespaceName, GetConfig());
    }

    private void UpdateFileProperties(Properties newProperties)
    {
        if (_baseDir == null) return;

        lock (this)
        {
            if (newProperties.Equals(_fileProperties)) return;

            _fileProperties = newProperties;

            PersistLocalCacheFile(_baseDir, Namespace);
        }
    }

    private Properties? LoadFromLocalCacheFile(string baseDir, string namespaceName)
    {
        if (string.IsNullOrWhiteSpace(baseDir))
            throw new ApolloConfigException("Basedir cannot be empty");

        var file = AssembleLocalCacheFile(baseDir, namespaceName);

        try
        {
            var properties = _options.CacheFileProvider.Get(file);

            Logger().Debug($"Loading local config file {file} successfully!");

            return properties;
        }
        catch (Exception ex)
        {
            throw new ApolloConfigException($"Loading config from local cache file {file} failed", ex);
        }
    }

    private void PersistLocalCacheFile(string? baseDir, string namespaceName)
    {
        var properties = _fileProperties;
        if (baseDir == null || properties == null) return;

        var file = AssembleLocalCacheFile(baseDir, namespaceName);

        try
        {
            _options.CacheFileProvider.Save(file, properties);
        }
        catch (Exception ex)
        {
            Logger().Warn($"Persist local cache file {file} failed, reason: {ex.GetDetailMessage()}.", ex);
        }
    }

    private void PrepareConfigCacheDir()
    {
        try
        {
            _baseDir = Path.Combine(_options.LocalCacheDir, ConfigDir);
        }
        catch (Exception ex)
        {
            Logger().Warn(new ApolloConfigException("Prepare config cache dir failed", ex));
            return;
        }

        CheckLocalConfigCacheDir(_baseDir);
    }

    private static void CheckLocalConfigCacheDir(string baseDir)
    {
        if (Directory.Exists(baseDir)) return;

        try
        {
            Directory.CreateDirectory(baseDir);
        }
        catch (Exception ex)
        {
            Logger()
                .Warn(
                    $"Unable to create local config cache directory {baseDir}, reason: {ex.GetDetailMessage()}. Will not able to cache config file.",
                    ex);
        }
    }

    private string AssembleLocalCacheFile(string baseDir, string namespaceName)
    {
        var fileName =
            $"{string.Join(ConfigConsts.ClusterNamespaceSeparator, _options.AppId, _options.Cluster, namespaceName)}.json";

        return Path.Combine(baseDir, fileName);
    }

    public override string ToString() => $"local {_options.AppId} {Namespace}";
}
