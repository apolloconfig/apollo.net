using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Model;
using System;

namespace Apollo.Configuration.Demo
{
    class ConfigurationManagerDemo
    {
        private string DEFAULT_VALUE = "undefined";
        private IConfig config;
        private IConfig anotherConfig;

        public ConfigurationManagerDemo(ApolloConfigurationManager configurationManager)
        {
            config = configurationManager.GetAppConfig();
            anotherConfig = configurationManager.GetConfig("TEST1.test");
            config.ConfigChanged += OnChanged;
            anotherConfig.ConfigChanged += OnChanged;
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
