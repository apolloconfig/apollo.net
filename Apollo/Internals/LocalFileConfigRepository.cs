using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;
using JetBrains.Annotations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class LocalFileConfigRepository : AbstractConfigRepository, IRepositoryChangeListener
    {
        private static readonly Func<Action<LogLevel, string, Exception>> Logger = () => LogManager.CreateLogger(typeof(LocalFileConfigRepository));
        private const string ConfigDir = "config-cache";

        private string _baseDir;
        [CanBeNull]
        private volatile Properties _fileProperties;

        private readonly IApolloOptions _options;
        private readonly IConfigRepository _upstream;

        public ConfigFileFormat Format { get; } = ConfigFileFormat.Properties;

        public LocalFileConfigRepository(string @namespace,
            IApolloOptions configUtil,
            IConfigRepository upstream = null) : base(@namespace)
        {
            _upstream = upstream;
            _options = configUtil;

            var ext = Path.GetExtension(@namespace);
            if (ext != null && ext.Length > 1 && Enum.TryParse(ext.Substring(1), true, out ConfigFileFormat format)) Format = format;

            PrepareConfigCacheDir();
        }

        public override async Task Initialize()
        {
            if (_upstream != null)
            {
                await _upstream.Initialize().ConfigureAwait(false);

                _upstream.AddChangeListener(this);

                //sync with upstream immediately
                if (TrySyncFromUpstream()) return;
            }

            try
            {
                _fileProperties = LoadFromLocalCacheFile(_baseDir, Namespace);
            }
            catch (Exception ex)
            {
                Logger().Warn(ex);
            }
        }

        public override Properties GetConfig()
        {
            var properties = _fileProperties == null ? new Properties() : new Properties(_fileProperties);

            return Format != ConfigFileFormat.Properties && ConfigAdapterRegister.TryGetAdapter(Format, out var adapter)
                ? adapter.GetProperties(properties)
                : properties;
        }

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _upstream?.Dispose();
            }

            //释放非托管资源

            _disposed = true;
        }

        private bool TrySyncFromUpstream()
        {
            if (_upstream == null) return false;

            try
            {
                var properties = _upstream.GetConfig();

                UpdateFileProperties(properties);

                return true;
            }
            catch (Exception ex)
            {
                Logger().Warn(
                    $"Sync config from upstream repository {_upstream.GetType()} failed, reason: {ex.GetDetailMessage()}");
            }

            return false;
        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            UpdateFileProperties(new Properties(newProperties));

            FireRepositoryChange(namespaceName, GetConfig());
        }

        private void UpdateFileProperties(Properties newProperties)
        {
            lock (this)
            {
                if (newProperties.Equals(_fileProperties)) return;

                _fileProperties = newProperties;

                PersistLocalCacheFile(_baseDir, Namespace);
            }
        }

        private Properties LoadFromLocalCacheFile(string baseDir, string namespaceName)
        {
            if (string.IsNullOrWhiteSpace(baseDir))
            {
                throw new ApolloConfigException("Basedir cannot be empty");
            }

            var file = AssembleLocalCacheFile(baseDir, namespaceName);

            try
            {
                var properties = new Properties(file);

                Logger().Debug($"Loading local config file {file} successfully!");

                return properties;
            }
            catch (Exception ex)
            {
                throw new ApolloConfigException($"Loading config from local cache file {file} failed", ex);
            }
        }

        private void PersistLocalCacheFile(string baseDir, string namespaceName)
        {
            var properties = _fileProperties;
            if (baseDir == null || properties == null) return;

            var file = AssembleLocalCacheFile(baseDir, namespaceName);

            try
            {
                properties.Store(file);
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

        private void CheckLocalConfigCacheDir(string baseDir)
        {
            if (Directory.Exists(baseDir)) return;

            try
            {
                Directory.CreateDirectory(baseDir);
            }
            catch (Exception ex)
            {
                Logger().Warn($"Unable to create local config cache directory {baseDir}, reason: {ex.GetDetailMessage()}. Will not able to cache config file.", ex);
            }
        }

        private string AssembleLocalCacheFile(string baseDir, string namespaceName)
        {
            var fileName = $"{string.Join(ConfigConsts.ClusterNamespaceSeparator, _options.AppId, _options.Cluster, namespaceName)}.json";

            return Path.Combine(baseDir, fileName);
        }
    }
}
