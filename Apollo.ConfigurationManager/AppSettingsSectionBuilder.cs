using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Model;
using Com.Ctrip.Framework.Apollo.Util;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Reflection.Emit;

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

                if (ForceUpdate)
                {
                    var dynamicMethod = new DynamicMethod("AppSettingsRemove",
                        typeof(void),
                        new[] { typeof(NameValueCollection), typeof(string) },
                        typeof(AppSettingsSectionBuilder).Module);

                    var il = dynamicMethod.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    // ReSharper disable once AssignNullToNotNullAttribute
                    il.Emit(OpCodes.Call, typeof(NameValueCollection).GetMethod("Remove"));
                    il.Emit(OpCodes.Ret);

                    //dynamicMethod.DefineParameter(0, ParameterAttributes.In, "appSettings");
                    //dynamicMethod.DefineParameter(1, ParameterAttributes.In, "name");

                    var remove = (Action<NameValueCollection, string>)dynamicMethod.CreateDelegate(typeof(Action<NameValueCollection, string>));

                    config.ConfigChanged += ConfigConfigChanged;

                    void ConfigConfigChanged(IConfig _, ConfigChangeEventArgs args)
                    {
                        lock (ConfigurationManager.AppSettings)
                        {
                            foreach (var change in args.Changes)
                            {
                                if (change.Value.ChangeType == PropertyChangeType.Deleted)
                                    remove(ConfigurationManager.AppSettings, change.Value.PropertyName);
                                else
                                    ConfigurationManager.AppSettings.Set(change.Value.PropertyName, change.Value.NewValue);
                            }
                        }
                    }
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
}
