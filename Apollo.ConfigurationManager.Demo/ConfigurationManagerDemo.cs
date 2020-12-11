using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Model;
using System;

namespace Apollo.ConfigurationManager.Demo
{
    internal class ConfigurationManagerDemo
    {
        private const string DefaultValue = "undefined";
        private readonly IConfig _config;

        public ConfigurationManagerDemo()
        {
            _config = ApolloConfigurationManager.GetConfig(ConfigConsts.NamespaceApplication + ".json",
                ConfigConsts.NamespaceApplication + ".xml",
                ConfigConsts.NamespaceApplication + ".yml",
                ConfigConsts.NamespaceApplication + ".yaml",
                ConfigConsts.NamespaceApplication).GetAwaiter().GetResult();

            _config.ConfigChanged += OnChanged;
        }

        public string GetConfig(string key)
        {
            var result = _config.GetProperty(key, DefaultValue);
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            Console.ForegroundColor = color;

            return result;
        }

        private static void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            foreach (var kv in changeEvent.Changes)
            {
                Console.WriteLine("Change - key: {0}, oldValue: {1}, newValue: {2}, changeType: {3}",
                    kv.Value.PropertyName, kv.Value.OldValue, kv.Value.NewValue, kv.Value.ChangeType);
            }
        }
    }
}
