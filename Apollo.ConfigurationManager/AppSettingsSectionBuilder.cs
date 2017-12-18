#if CONFIGURATIONBUILDER
using Com.Ctrip.Framework.Apollo.Util;
using System.Collections.Specialized;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo
{
    public class AppSettingsSectionBuilder : ApolloConfigurationBuilder
    {
        private string _keyPrefix;
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _keyPrefix = config["keyPrefix"] ?? Namespace;
        }

        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            if (configSection is AppSettingsSection section)
            {
                var appSettings = section.Settings;

                TrySetConfigUtil(appSettings);

                lock (this)
                {
                    var config = GetConfig();
                    foreach (var name in config.GetPropertyNames())
                    {
                        var value = config.GetProperty(name, null);

                        var key = string.IsNullOrEmpty(_keyPrefix) ? name : $"{_keyPrefix}:{name}";

                        if (!string.IsNullOrEmpty(value))
                            appSettings.Remove(key);

                        appSettings.Add(key, value);
                    }
                }
            }

            return base.ProcessConfigurationSection(configSection);
        }

        private static bool TrySetConfigUtil(KeyValueConfigurationCollection appSettings)
        {
            if (ConfigUtil.AppSettings == null)
            {
                var coll = new NameValueCollection();

                foreach (var key in appSettings.AllKeys)
                {
                    coll.Add(key, appSettings[key].Value);
                }

                ConfigUtil.AppSettings = coll;

                return true;
            }

            return false;
        }
    }
}
#endif
