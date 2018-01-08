using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Model;
using System;

namespace ApolloDemo
{
    class ConfigurationManagerDemo
    {
        private string DEFAULT_VALUE = "undefined";
        private IConfig config;
        private IConfig anotherConfig;

        public ConfigurationManagerDemo()
        {
            config = ApolloConfigurationManager.GetAppConfig();
            anotherConfig = ApolloConfigurationManager.GetConfig("TEST1.test");
            config.ConfigChanged += OnChanged;
            anotherConfig.ConfigChanged += OnChanged;
        }

        public string GetConfig(string key)
        {
            string result = config.GetProperty(key, DEFAULT_VALUE);
            if (result.Equals(DEFAULT_VALUE))
            {
                result = anotherConfig.GetProperty(key, DEFAULT_VALUE);
            }
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            Console.ForegroundColor = color;

            return result;
        }

        private void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            Console.WriteLine("Changes for namespace {0}", changeEvent.Namespace);
            foreach (var change in changeEvent.Changes)
            {
                Console.WriteLine("Change - key: {0}, oldValue: {1}, newValue: {2}, changeType: {3}",
                    change.Value.PropertyName, change.Value.OldValue, change.Value.NewValue, change.Value.ChangeType);
            }
        }
    }
}
