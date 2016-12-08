using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Core;
using System.IO;
using Com.Ctrip.Framework.Apollo.Exceptions;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class LocalFileConfigRepository : AbstractConfigRepository, RepositoryChangeListener
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LocalFileConfigRepository));
        private const string CONFIG_DIR = "config-cache";
        private readonly string m_namespace;
        private string m_baseDir;
        private readonly ConfigUtil m_configUtil;
        private volatile Properties m_fileProperties;
        private volatile ConfigRepository m_upstream;

        public LocalFileConfigRepository(string namespaceName) : this(namespaceName, null)
        {
        }

        public LocalFileConfigRepository(string namespaceName, ConfigRepository upstream)
        {
            m_namespace = namespaceName;
            m_configUtil = ComponentLocator.Lookup<ConfigUtil>();
            this.PrepareConfigCacheDir();
            this.SetUpstreamRepository(upstream);
            this.TrySync();
        }

        protected override void Sync()
        {
            //sync with upstream immediately
            bool syncFromUpstreamResultSuccess = TrySyncFromUpstream();

            if (syncFromUpstreamResultSuccess)
            {
                return;
            }

            Exception exception = null;
            try
            {
                m_fileProperties = this.LoadFromLocalCacheFile(m_baseDir, m_namespace);
            }
            catch (Exception ex)
            {
                logger.Warn(ex);
                exception = ex;
                //ignore
            }

            if (m_fileProperties == null)
            {
                throw new ApolloConfigException("Load config from local config failed!", exception);
            }

        }

        public override Properties GetConfig()
        {
            if (m_fileProperties == null)
            {
                Sync();
            }
            Properties result = new Properties(m_fileProperties);
            return result;
        }

        public override void SetUpstreamRepository(ConfigRepository upstreamConfigRepository)
        {
            if (upstreamConfigRepository == null)
            {
                return;
            }
            //clear previous listener
            if (m_upstream != null)
            {
                m_upstream.RemoveChangeListener(this);
            }
            m_upstream = upstreamConfigRepository;
            TrySyncFromUpstream();
            upstreamConfigRepository.AddChangeListener(this);
        }

        private bool TrySyncFromUpstream()
        {
            if (m_upstream == null)
            {
                return false;
            }
            try
            {
                Properties properties = m_upstream.GetConfig();
                UpdateFileProperties(properties);
                return true;
            }
            catch (Exception ex)
            {
                logger.Warn(
                    string.Format("Sync config from upstream repository {0} failed, reason: {1}",
                    m_upstream.GetType(), ExceptionUtil.GetDetailMessage(ex)));
            }

            return false;
        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            Properties newFileProperties = new Properties(newProperties);
            UpdateFileProperties(newFileProperties);
            this.FireRepositoryChange(namespaceName, newProperties);
        }

        private void UpdateFileProperties(Properties newProperties)
        {
            lock (this)
            {
                if (newProperties.Equals(m_fileProperties))
                {
                    return;
                }
                this.m_fileProperties = newProperties;
                PersistLocalCacheFile(m_baseDir, m_namespace);
            }
        }

        private Properties LoadFromLocalCacheFile(string baseDir, string namespaceName)
        {
            if (string.IsNullOrWhiteSpace(baseDir))
            {
                throw new ApolloConfigException("Basedir cannot be empty");
            }

            string file = AssembleLocalCacheFile(baseDir, namespaceName);
            Properties properties = new Properties();

            try
            {
                properties.Load(file);
                logger.Debug(string.Format("Loading local config file {0} successfully!", file));
            }
            catch (Exception ex)
            {
                throw new ApolloConfigException(string.Format("Loading config from local cache file {0} failed", file), ex);
            }

            return properties;
        }

        private void PersistLocalCacheFile(string baseDir, string namespaceName)
        {
            if (baseDir == null)
            {
                return;
            }
            string file = AssembleLocalCacheFile(baseDir, namespaceName);

            try
            {
                m_fileProperties.Store(file);
            }
            catch (Exception ex)
            {
                ApolloConfigException exception = new ApolloConfigException(
                    string.Format("Persist local cache file {0} failed", file), ex);
                logger.Warn(
                    string.Format("Persist local cache file {0} failed, reason: {1}.",
                    file, ExceptionUtil.GetDetailMessage(ex)));
            }
        }

        private void PrepareConfigCacheDir()
        {
            try
            {
                m_baseDir = Path.Combine(m_configUtil.DefaultLocalCacheDir, CONFIG_DIR);
            }
            catch (Exception ex)
            {
                logger.Warn(new ApolloConfigException("Prepare config cache dir failed", ex));
                return;
            }
            this.CheckLocalConfigCacheDir(m_baseDir);
        }

        private void CheckLocalConfigCacheDir(string baseDir)
        {
            if (Directory.Exists(baseDir))
            {
                return;
            }
 
            try
            {
                Directory.CreateDirectory(baseDir);
            }
            catch (Exception ex)
            {
                ApolloConfigException exception = new ApolloConfigException(
                    string.Format("Create local config directory {0} failed", baseDir), ex);
                logger.Warn(
                    string.Format("Unable to create local config cache directory {0}, reason: {1}. Will not able to cache config file.", baseDir, ExceptionUtil.GetDetailMessage(ex)));
            }
        }

        private string AssembleLocalCacheFile(string baseDir, string namespaceName)
        {
            string fileName = string.Format("{0}.json",
                string.Join(ConfigConsts.CLUSTER_NAMESPACE_SEPARATOR,
                new string[] { m_configUtil.AppId, m_configUtil.Cluster, namespaceName }));

            return Path.Combine(baseDir, fileName);
        }

    }
}
