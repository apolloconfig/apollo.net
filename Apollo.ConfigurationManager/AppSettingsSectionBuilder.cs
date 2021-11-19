﻿using Com.Ctrip.Framework.Apollo.Util;
using System.Configuration;

namespace Com.Ctrip.Framework.Apollo;

public class AppSettingsSectionBuilder : ApolloConfigurationBuilder
{
    private string? _keyPrefix;

    public override void Initialize(string name, NameValueCollection config)
    {
        base.Initialize(name, config);

        _keyPrefix = config["keyPrefix"];

        if (!string.IsNullOrWhiteSpace(_keyPrefix) && !_keyPrefix.EndsWith(":"))
            _keyPrefix += ":";
    }

    public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
    {
        if (configSection is not AppSettingsSection section) return base.ProcessConfigurationSection(configSection);

        var appSettings = section.Settings;

        TrySetConfigUtil(appSettings);

        lock (this)
        {
            var config = GetConfig().WithPrefix(_keyPrefix);

            foreach (var key in config.GetPropertyNames())
            {
                if (config.TryGetProperty(key, out var value))
                    appSettings.Remove(key);

                appSettings.Add(key, value);
            }
        }

        return base.ProcessConfigurationSection(configSection);
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
