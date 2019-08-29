using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Model;
using Com.Ctrip.Framework.Apollo.Util;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo
{
    public class AppSettingsSectionBuilder : ApolloConfigurationBuilder
    {
        public bool ForceUpdate { get; private set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            ForceUpdate = !string.Equals("false", config["forceUpdate"], StringComparison.OrdinalIgnoreCase);

            base.Initialize(name, config);
        }

        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            if (!(configSection is AppSettingsSection section)) return base.ProcessConfigurationSection(configSection);

            var appSettings = section.Settings;

            TrySetConfigUtil(appSettings);

            lock (this)
            {
                var config = GetConfig();
                foreach (var key in config.GetPropertyNames())
                {
                    if (config.TryGetProperty(key, out var value) && !string.IsNullOrEmpty(value))
                        appSettings.Remove(key);

                    appSettings.Add(key, value);
                }

                if (ForceUpdate) config.ConfigChanged += Config_ConfigChanged;
            }

            return base.ProcessConfigurationSection(configSection);
        }

        private void Config_ConfigChanged(object sender, ConfigChangeEventArgs args)
        {
            lock (ConfigurationManager.AppSettings)
            {
                foreach (var change in args.Changes)
                {
                    if (change.Value.ChangeType == PropertyChangeType.Deleted)
                        ConfigurationManager.AppSettings.Remove(change.Value.PropertyName);
                    else
                        ConfigurationManager.AppSettings.Set(change.Value.PropertyName, change.Value.NewValue);
                }
            }
        }

        private static void TrySetConfigUtil(KeyValueConfigurationCollection appSettings)
        {
            if (ConfigUtil.AppSettings != null) return;

            var settings = new NameValueCollection();

            foreach (var key in appSettings.AllKeys)
                settings.Add(key, appSettings[key].Value);

            ConfigUtil.AppSettings = settings;
        }
    }
}
