using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Com.Ctrip.Framework.Apollo.Internals
{
    public class MultiConfig : AbstractConfig
    {
        private readonly IReadOnlyCollection<IConfig> _configs;

        public override event ConfigChangeEvent ConfigChanged
        {
            add
            {
                foreach (var config in _configs)
                    config.ConfigChanged += value;
            }
            remove
            {
                foreach (var config in _configs)
                    config.ConfigChanged -= value;
            }
        }

        /// <param name="configs">order desc</param>
        public MultiConfig([NotNull] IEnumerable<IConfig> configs)
        {
            if (configs == null) throw new ArgumentNullException(nameof(configs));

            _configs = configs.Reverse().ToArray();
        }

        public override string GetProperty(string key, string defaultValue)
        {
            foreach (var config in _configs)
            {
                var value = config.GetProperty(key, null);
                if (value != null)
                    return value;
            }

            return defaultValue;
        }

        public override ISet<string> GetPropertyNames()
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (_configs == null) return names;

            foreach (var config in _configs)
            {
                foreach (var propertyName in config.GetPropertyNames())
                {
                    names.Add(propertyName);
                }
            }

            return names;
        }
    }
}
