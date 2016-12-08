using System;
using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Model;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging.Spi;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class DefaultConfig : AbstractConfig, RepositoryChangeListener
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DefaultConfig));
        private readonly string m_namespace;
        private ThreadSafe.AtomicReference<Properties> m_configProperties;
        private ConfigRepository m_configRepository;

        public DefaultConfig(string namespaceName, ConfigRepository configRepository)
        {
            m_namespace = namespaceName;
	        m_configRepository = configRepository;
            m_configProperties = new ThreadSafe.AtomicReference<Properties>(null);
	        Initialize();
        }

        private void Initialize()
        {
            try
            {
                m_configProperties.WriteFullFence(m_configRepository.GetConfig());
            }
            catch (Exception ex)
            {
                logger.Warn(
                    string.Format("Init Apollo Local Config failed - namespace: {0}, reason: {1}.", 
                    m_namespace, ExceptionUtil.GetDetailMessage(ex)));
            }
            finally
            {
                //register the change listener no matter config repository is working or not
                //so that whenever config repository is recovered, config could get changed
                m_configRepository.AddChangeListener(this);
            }
        }

        public override string GetProperty(String key, string defaultValue) {
            // step 1: check system properties, i.e. -Dkey=value
            //TODO looks like .Net doesn't have such system property?
            string value = null;

            // step 2: check local cached properties file
            if (m_configProperties.ReadFullFence() != null)
            {
                value = m_configProperties.ReadFullFence().GetProperty(key);
            }

            /// <summary>
            /// step 3: check env variable, i.e. PATH=...
            /// normally system environment variables are in UPPERCASE, however there might be exceptions.
            /// so the caller should provide the key in the right case
            /// </summary>
            if (value == null)
            {
                value = System.Environment.GetEnvironmentVariable(key);
            }

            //TODO step 4: check properties file from classpath
            

            if (value == null && m_configProperties.ReadFullFence() == null)
            {
                logger.Warn(string.Format("Could not load config for namespace {0} from Apollo, please check whether the configs are released in Apollo! Return default value now!", m_namespace));
            }

            return value == null ? defaultValue : value;

        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            lock (this)
            {
                Properties newConfigProperties = new Properties(newProperties);

                IDictionary<string, ConfigChange> actualChanges = UpdateAndCalcConfigChanges(newConfigProperties);

                //check double checked result
                if (actualChanges.Count == 0)
                {
                    return;
                }

                this.FireConfigChange(new ConfigChangeEventArgs(m_namespace, actualChanges));
            }
        }

        private IDictionary<string, ConfigChange> UpdateAndCalcConfigChanges(Properties newConfigProperties)
        {
            ICollection<ConfigChange> configChanges = CalcPropertyChanges(m_namespace, m_configProperties.ReadFullFence(), newConfigProperties);

            IDictionary<string, ConfigChange> actualChanges = new Dictionary<string, ConfigChange>();

            /// <summary>
            /// === Double check since DefaultConfig has multiple config sources ==== </summary>

            //1. use getProperty to update configChanges's old value
            foreach (ConfigChange change in configChanges)
            {
                change.OldValue = this.GetProperty(change.PropertyName, change.OldValue);
            }

            //2. update m_configProperties
            m_configProperties.WriteFullFence(newConfigProperties);

            //3. use getProperty to update configChange's new value and calc the final changes
            foreach (ConfigChange change in configChanges)
            {
                change.NewValue = this.GetProperty(change.PropertyName, change.NewValue);
                switch (change.ChangeType)
                {
                    case PropertyChangeType.ADDED:
                        if (string.Equals(change.OldValue, change.NewValue))
                        {
                            break;
                        }
                        if (change.OldValue != null)
                        {
                            change.ChangeType = PropertyChangeType.MODIFIED;
                        }
                        actualChanges[change.PropertyName] = change;
                        break;
                    case PropertyChangeType.MODIFIED:
                        if (!string.Equals(change.OldValue, change.NewValue))
                        {
                            actualChanges[change.PropertyName] = change;
                        }
                        break;
                    case PropertyChangeType.DELETED:
                        if (string.Equals(change.OldValue, change.NewValue))
                        {
                            break;
                        }
                        if (change.NewValue != null)
                        {
                            change.ChangeType = PropertyChangeType.MODIFIED;
                        }
                        actualChanges[change.PropertyName] = change;
                        break;
                    default:
                        //do nothing
                        break;
                }
            }
            return actualChanges;
        }

        public override ISet<string> GetPropertyNames()
        {
            Properties properties = m_configProperties.ReadFullFence();
            if (properties == null)
            {
                return new HashSet<string>();
            }

            return properties.GetPropertyNames();
        }
    }
}

