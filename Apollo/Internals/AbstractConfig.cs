using System;
using System.Collections.Generic;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Logging.Spi;
using Com.Ctrip.Framework.Apollo.Model;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;
using System.Linq;
using System.Text.RegularExpressions;
using Com.Ctrip.Framework.Apollo.Amib.Threading;
using Com.Ctrip.Framework.Apollo.Util;
using Com.Ctrip.Framework.Apollo.Exceptions;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public abstract class AbstractConfig : Config
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AbstractConfig));
        public event ConfigChangeEvent ConfigChanged;
        private static SmartThreadPool m_executorService;

        static AbstractConfig()
        {
            m_executorService = ThreadPoolUtil.NewThreadPool(1, 5, SmartThreadPool.DefaultIdleTimeout, true);
        }

        public abstract string GetProperty(string key, string defaultValue);

        public int? GetIntProperty(string key, int? defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                return value == null ? defaultValue : int.Parse(value);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetIntProperty for {0} failed, return default value {1:D}", key, defaultValue), ex));
                return defaultValue;
            }
        }

        public long? GetLongProperty(string key, long? defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                return value == null ? defaultValue : long.Parse(value);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetLongProperty for {0} failed, return default value {1:D}", key, defaultValue), ex));
                return defaultValue;
            }
        }

        public short? GetShortProperty(string key, short? defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                return value == null ? defaultValue : short.Parse(value);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetShortProperty for {0} failed, return default value {1:D}", key, defaultValue), ex));
                return defaultValue;
            }
        }

        public float? GetFloatProperty(string key, float? defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                return value == null ? defaultValue : float.Parse(value);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetFloatProperty for {0} failed, return default value {1:F}", key, defaultValue), ex));
                return defaultValue;
            }
        }

        public double? GetDoubleProperty(string key, double? defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                return value == null ? defaultValue : double.Parse(value);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetDoubleProperty for {0} failed, return default value {1:F}", key, defaultValue), ex));
                return defaultValue;
            }
        }

        public sbyte? GetByteProperty(string key, sbyte? defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                return value == null ? defaultValue : sbyte.Parse(value);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetByteProperty for {0} failed, return default value {1:G}", key, defaultValue), ex));
                return defaultValue;
            }
        }

        public bool? GetBooleanProperty(string key, bool? defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                return value == null ? defaultValue : bool.Parse(value);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetBooleanProperty for {0} failed, return default value {1}", key, defaultValue), ex));
                return defaultValue;
            }
        }

        public string[] GetArrayProperty(string key, string delimiter, string[] defaultValue)
        {
            try
            {
                string value = GetProperty(key, null);
                if (value == null)
                {
                    return defaultValue;
                }

                return Regex.Split(value, delimiter);
            }
            catch (Exception ex)
            {
                logger.Error(new ApolloConfigException(string.Format("GetArrayProperty for {0} failed, return default value", key), ex));
                return defaultValue;
            }
        }

        public abstract ISet<string> GetPropertyNames();

        protected void FireConfigChange(ConfigChangeEventArgs changeEvent)
        {
            if (ConfigChanged != null)
            {
                foreach (ConfigChangeEvent handler in ConfigChanged.GetInvocationList())
                {
                    m_executorService.QueueWorkItem((handlerCopy, changeEventCopy) =>
                        {
                            string methodName;
                            if (handlerCopy.Target == null)
                            {
                                methodName = handlerCopy.Method.Name;
                            }
                            else
                            {
                                methodName = string.Format("{0}.{1}", handlerCopy.Target.GetType(), handlerCopy.Method.Name);
                            }
                            try
                            {
                                handlerCopy(this, changeEventCopy);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(string.Format("Failed to invoke config change handler {0}", methodName), ex);
                            }
                        }, handler, changeEvent);
                }
            }
        }

        protected ICollection<ConfigChange> CalcPropertyChanges(string namespaceName, Properties previous, Properties current)
        {
            if (previous == null)
            {
                previous = new Properties();
            }

            if (current == null)
            {
                current = new Properties();
            }

            ISet<string> previousKeys = previous.GetPropertyNames();
            ISet<string> currentKeys = current.GetPropertyNames();

            IEnumerable<string> commonKeys = previousKeys.Intersect(currentKeys);
            IEnumerable<string> newKeys = currentKeys.Except(commonKeys);
            IEnumerable<string> removedKeys = previousKeys.Except(commonKeys);

            ICollection<ConfigChange> changes = new LinkedList<ConfigChange>();

            foreach (string newKey in newKeys)
            {
                changes.Add(new ConfigChange(namespaceName, newKey, null, current.GetProperty(newKey), PropertyChangeType.ADDED));
            }

            foreach (string removedKey in removedKeys)
            {
                changes.Add(new ConfigChange(namespaceName, removedKey, previous.GetProperty(removedKey), null, PropertyChangeType.DELETED));
            }

            foreach (string commonKey in commonKeys)
            {
                string previousValue = previous.GetProperty(commonKey);
                string currentValue = current.GetProperty(commonKey);
                if (string.Equals(previousValue, currentValue))
                {
                    continue;
                }
                changes.Add(new ConfigChange(namespaceName, commonKey, previousValue, currentValue, PropertyChangeType.MODIFIED));
            }

            return changes;
        }

    }
}

