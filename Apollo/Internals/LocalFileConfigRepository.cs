using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util;
using System;
using System.IO;
using System.Threading;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class LocalFileConfigRepository : AbstractConfigRepository, IRepositoryChangeListener
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LocalFileConfigRepository));
        private const string ConfigDir = "config-cache";

        private string _baseDir;
        private volatile Properties _fileProperties;

        private readonly IApolloOptions _options;
        private readonly IConfigRepository _upstream;
        private readonly ManualResetEventSlim _resetEvent = new ManualResetEventSlim(false);

        public LocalFileConfigRepository(string @namespace,
            IApolloOptions configUtil,
            IConfigRepository upstream = null) : base(@namespace)
        {
            _upstream = upstream;
            _options = configUtil;

            PrepareConfigCacheDir();

            Sync();
        }

        private void Sync()
        {
            if (_upstream != null)
            {
                _upstream.AddChangeListener(this);

                //sync with upstream immediately
                if (TrySyncFromUpstream())
                    return;
            }

            try
            {
                _fileProperties = LoadFromLocalCacheFile(_baseDir, Namespace);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }
        }

        public override Properties GetConfig()
        {
            if (_fileProperties == null)
                _resetEvent.Wait();

            return new Properties(_fileProperties);
        }

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _resetEvent.Dispose();
                _upstream?.Dispose();
            }

            //释放非托管资源

            _disposed = true;
        }

        private bool TrySyncFromUpstream()
        {
            if (_upstream == null)
                return false;

            try
            {
                var properties = _upstream.GetConfig();

                UpdateFileProperties(properties);

                if (!_resetEvent.IsSet)
                    _resetEvent.Set();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"Sync config from upstream repository {_upstream.GetType()} failed, reason: {ExceptionUtil.GetDetailMessage(ex)}");
            }

            return false;
        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            UpdateFileProperties(new Properties(newProperties));

            FireRepositoryChange(namespaceName, newProperties);
        }

        private void UpdateFileProperties(Properties newProperties)
        {
            lock (this)
            {
                if (newProperties.Equals(_fileProperties))
                    return;

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
            var properties = new Properties();

            try
            {
                properties.Load(file);
                if (!_resetEvent.IsSet)
                    _resetEvent.Set();

                Logger.Debug($"Loading local config file {file} successfully!");
            }
            catch (Exception ex)
            {
                throw new ApolloConfigException($"Loading config from local cache file {file} failed", ex);
            }

            return properties;
        }

        private void PersistLocalCacheFile(string baseDir, string namespaceName)
        {
            if (baseDir == null)
            {
                return;
            }
            var file = AssembleLocalCacheFile(baseDir, namespaceName);

            try
            {
                _fileProperties.Store(file);
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"Persist local cache file {file} failed, reason: {ExceptionUtil.GetDetailMessage(ex)}.", ex);
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
                Logger.Warn(new ApolloConfigException("Prepare config cache dir failed", ex));
                return;
            }
            CheckLocalConfigCacheDir(_baseDir);
        }

        private void CheckLocalConfigCacheDir(string baseDir)
        {
            if (Directory.Exists(baseDir))
                return;

            try
            {
                Directory.CreateDirectory(baseDir);
            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"Unable to create local config cache directory {baseDir}, reason: {ExceptionUtil.GetDetailMessage(ex)}. Will not able to cache config file.", ex);
            }
        }

        private string AssembleLocalCacheFile(string baseDir, string namespaceName)
        {
            var fileName = $"{string.Join(ConfigConsts.ClusterNamespaceSeparator, _options.AppId, _options.Cluster, namespaceName)}.json";

            return Path.Combine(baseDir, fileName);
        }
    }
}
