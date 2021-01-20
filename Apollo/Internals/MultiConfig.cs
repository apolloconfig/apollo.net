using Com.Ctrip.Framework.Apollo.Core.Utils;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class MultiConfig : AbstractConfig
    {
#if NET40
        private readonly ICollection<IConfig> _configs;
#else
        private readonly IReadOnlyCollection<IConfig> _configs;
#endif
        private Properties _configProperties;

        /// <param name="configs">order desc</param>
        public MultiConfig(IEnumerable<IConfig> configs)
        {
            if (configs == null) throw new ArgumentNullException(nameof(configs));

            _configs = configs.ToArray();

            foreach (var config in _configs)
            {
                config.ConfigChanged += Config_ConfigChanged;
            }

            _configProperties = CombineProperties();
        }

        private Properties CombineProperties()
        {
            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var config in _configs)
                foreach (var name in config.GetPropertyNames())
                {
                    if (!dic.ContainsKey(name) && config.TryGetProperty(name, out var value)) dic[name] = value;
                }

            return new Properties(dic);
        }

        public override bool TryGetProperty(string key, [NotNullWhen(true)] out string? value) =>
            _configProperties.TryGetProperty(key, out value);

        public override IEnumerable<string> GetPropertyNames() => _configProperties.GetPropertyNames();

        private void Config_ConfigChanged(object sender, ConfigChangeEventArgs args)
        {
            lock (this)
            {
                var newConfigProperties = CombineProperties();

                var actualChanges = UpdateAndCalcConfigChanges(newConfigProperties);

                //check double checked result
                if (actualChanges.Count == 0) return;

                FireConfigChange(actualChanges);
            }
        }
#if NET40
        private IDictionary<string, ConfigChange> UpdateAndCalcConfigChanges(Properties newConfigProperties)
#else
        private IReadOnlyDictionary<string, ConfigChange> UpdateAndCalcConfigChanges(Properties newConfigProperties)
#endif
        {
            var configChanges = CalcPropertyChanges(_configProperties, newConfigProperties);

            var actualChanges = new Dictionary<string, ConfigChange>();

            //1. use getProperty to update configChanges's old value
            foreach (var change in configChanges)
            {
                change.OldValue = this.GetProperty(change.PropertyName, change.OldValue);
            }

            //2. update _configProperties
            _configProperties = newConfigProperties;

            //3. use getProperty to update configChange's new value and calc the final changes
            foreach (var change in configChanges)
            {
                change.NewValue = this.GetProperty(change.PropertyName, change.NewValue);
                switch (change.ChangeType)
                {
                    case PropertyChangeType.Added:
                        if (string.Equals(change.OldValue, change.NewValue))
                        {
                            break;
                        }
                        if (change.OldValue != null)
                        {
                            change.ChangeType = PropertyChangeType.Modified;
                        }
                        actualChanges[change.PropertyName] = change;
                        break;
                    case PropertyChangeType.Modified:
                        if (!string.Equals(change.OldValue, change.NewValue))
                        {
                            actualChanges[change.PropertyName] = change;
                        }
                        break;
                    case PropertyChangeType.Deleted:
                        if (string.Equals(change.OldValue, change.NewValue))
                        {
                            break;
                        }
                        if (change.NewValue != null)
                        {
                            change.ChangeType = PropertyChangeType.Modified;
                        }
                        actualChanges[change.PropertyName] = change;
                        break;
                }
            }
            return actualChanges;
        }
    }
}
