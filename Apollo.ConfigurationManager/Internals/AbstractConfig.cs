using Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Exceptions;
using Com.Ctrip.Framework.Apollo.Logging;
using Com.Ctrip.Framework.Apollo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public abstract class AbstractConfig : IConfig
    {
        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(AbstractConfig));
        public event ConfigChangeEvent ConfigChanged;
        private static readonly TaskFactory ExecutorService;

        static AbstractConfig()
        {
            ExecutorService = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(5));
        }

        public abstract string GetProperty(string key, string defaultValue);

        public int? GetIntProperty(string key, int? defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                return value == null ? defaultValue : int.Parse(value);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetIntProperty for {key} failed, return default value {defaultValue:D}", ex));
                return defaultValue;
            }
        }

        public long? GetLongProperty(string key, long? defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                return value == null ? defaultValue : long.Parse(value);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetLongProperty for {key} failed, return default value {defaultValue:D}", ex));
                return defaultValue;
            }
        }

        public short? GetShortProperty(string key, short? defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                return value == null ? defaultValue : short.Parse(value);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetShortProperty for {key} failed, return default value {defaultValue:D}", ex));
                return defaultValue;
            }
        }

        public float? GetFloatProperty(string key, float? defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                return value == null ? defaultValue : float.Parse(value);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetFloatProperty for {key} failed, return default value {defaultValue:F}", ex));
                return defaultValue;
            }
        }

        public double? GetDoubleProperty(string key, double? defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                return value == null ? defaultValue : double.Parse(value);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetDoubleProperty for {key} failed, return default value {defaultValue:F}", ex));
                return defaultValue;
            }
        }

        public sbyte? GetByteProperty(string key, sbyte? defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                return value == null ? defaultValue : sbyte.Parse(value);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetByteProperty for {key} failed, return default value {defaultValue:G}", ex));
                return defaultValue;
            }
        }

        public bool? GetBooleanProperty(string key, bool? defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                return value == null ? defaultValue : bool.Parse(value);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetBooleanProperty for {key} failed, return default value {defaultValue}", ex));
                return defaultValue;
            }
        }

        public string[] GetArrayProperty(string key, string delimiter, string[] defaultValue)
        {
            try
            {
                var value = GetProperty(key, null);
                if (value == null)
                {
                    return defaultValue;
                }

                return Regex.Split(value, delimiter);
            }
            catch (Exception ex)
            {
                Logger.Error(new ApolloConfigException($"GetArrayProperty for {key} failed, return default value", ex));
                return defaultValue;
            }
        }

        public abstract ISet<string> GetPropertyNames();

        protected void FireConfigChange(ConfigChangeEventArgs changeEventCopy)
        {
            if (ConfigChanged != null)
            {
                foreach (var @delegate in ConfigChanged.GetInvocationList())
                {
                    var handlerCopy = (ConfigChangeEvent) @delegate;
                    ExecutorService.StartNew(() =>
                    {
                        string methodName;
                        if (handlerCopy.Target == null)
                        {
                            methodName = handlerCopy.Method.Name;
                        }
                        else
                        {
                            methodName = $"{handlerCopy.Target.GetType()}.{handlerCopy.Method.Name}";
                        }

                        try
                        {
                            handlerCopy(this, changeEventCopy);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Failed to invoke config change handler {methodName}", ex);
                        }
                    });
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

            var previousKeys = previous.GetPropertyNames();
            var currentKeys = current.GetPropertyNames();

            var commonKeys = previousKeys.Intersect(currentKeys);
            var newKeys = currentKeys.Except(commonKeys);
            var removedKeys = previousKeys.Except(commonKeys);

            ICollection<ConfigChange> changes = new LinkedList<ConfigChange>();

            foreach (var newKey in newKeys)
            {
                changes.Add(new ConfigChange(namespaceName, newKey, null, current.GetProperty(newKey), PropertyChangeType.Added));
            }

            foreach (var removedKey in removedKeys)
            {
                changes.Add(new ConfigChange(namespaceName, removedKey, previous.GetProperty(removedKey), null, PropertyChangeType.Deleted));
            }

            foreach (var commonKey in commonKeys)
            {
                var previousValue = previous.GetProperty(commonKey);
                var currentValue = current.GetProperty(commonKey);
                if (string.Equals(previousValue, currentValue))
                {
                    continue;
                }
                changes.Add(new ConfigChange(namespaceName, commonKey, previousValue, currentValue, PropertyChangeType.Modified));
            }

            return changes;
        }
    }
}

