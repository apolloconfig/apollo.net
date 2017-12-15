using Com.Ctrip.Framework.Apollo.Model;
using Com.Ctrip.Framework.Apollo.Util;
using System.Collections.Specialized;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo
{
    public class AppSettingsSectionBuilder : ConfigurationBuilder
    {
        private IConfig _config;

        private string _namespace;
        public override void Initialize(string name, NameValueCollection config)
        {
            _namespace = config["namespace"];

            base.Initialize(name, config);
        }

        public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
        {
            if (configSection is AppSettingsSection section)
            {
                var appSettings = section.Settings;

                if (_config == null)
                    _config = GetConfig(appSettings);

                lock (this)
                    foreach (var name in _config.GetPropertyNames())
                    {
                        var value = _config.GetProperty(name, null);

                        if (!string.IsNullOrEmpty(value))
                            appSettings.Remove(name);

                        appSettings.Add(name, value);
                    }
            }

            return base.ProcessConfigurationSection(configSection);
        }

        private IConfig GetConfig(KeyValueConfigurationCollection appSettings)
        {
            if (ConfigUtil.AppSettings == null)
            {
                var coll = new NameValueCollection();

                foreach (var key in appSettings.AllKeys)
                {
                    coll.Add(key, appSettings[key].Value);
                }

                ConfigUtil.AppSettings = coll;
            }

            var config = _namespace == null ? ConfigManager.GetAppConfig() : ConfigManager.GetConfig(_namespace);

            config.ConfigChanged += Config_ConfigChanged;

            return config;
        }

        private void Config_ConfigChanged(object sender, ConfigChangeEventArgs args) => ConfigurationManager.RefreshSection("appSettings");
    }
}
