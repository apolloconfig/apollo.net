using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Core;
using Com.Ctrip.Framework.Apollo.Model;
using System;

namespace Apollo.ConfigurationManager.Demo
{
    class ConfigurationManagerDemo
    {
        private readonly string DEFAULT_VALUE = "undefined";
        private readonly IConfig config;

        public ConfigurationManagerDemo()
        {
            config = ApolloConfigurationManager.GetConfig(ConfigConsts.NamespaceApplication + ".json",
                ConfigConsts.NamespaceApplication + ".xml",
                ConfigConsts.NamespaceApplication + ".yml",
                ConfigConsts.NamespaceApplication + ".yaml",
                ConfigConsts.NamespaceApplication).GetAwaiter().GetResult();
            config.ConfigChanged += OnChanged;
        }

        public string GetConfig(string key)
        {
            var result = config.GetProperty(key, DEFAULT_VALUE);
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            Console.ForegroundColor = color;

            return result;
        }

        private void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            Console.WriteLine("Changes for namespace {0}", changeEvent.Namespace);
            foreach (var (key, value) in changeEvent.Changes)
            {
                Console.WriteLine("Change - key: {0}, oldValue: {1}, newValue: {2}, changeType: {3}",
                    value.PropertyName, value.OldValue, value.NewValue, value.ChangeType);
            }
        }
    }
}
