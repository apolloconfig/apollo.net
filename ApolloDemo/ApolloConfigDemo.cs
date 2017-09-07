using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Model;
using Com.Ctrip.Framework.Foundation;
using System;

namespace ApolloDemo
{
    class ApolloConfigDemo
    {
        private string DEFAULT_VALUE = "undefined";
        private Config config;
        private Config anotherConfig;

        public ApolloConfigDemo()
        {
            config = ConfigService.GetAppConfig();
            anotherConfig = ConfigService.GetConfig("FX.apollo");
            config.ConfigChanged += new ConfigChangeEvent(OnChanged);
            anotherConfig.ConfigChanged += new ConfigChangeEvent(OnChanged);
        }

        private string GetConfig(string key)
        {
            string result = config.GetProperty(key, DEFAULT_VALUE);
            if (result.Equals(DEFAULT_VALUE))
            {
                result = anotherConfig.GetProperty(key, DEFAULT_VALUE);
            }
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            return result;
        }

        public static void Main(string[] args)
        {
            ApolloConfigDemo apolloConfigDemo = new ApolloConfigDemo();
            apolloConfigDemo.PrintEnvInfo();
	        Console.WriteLine("Apollo Config Demo. Please input key to get the value. Input quit to exit.");
	        while (true)
	        {
	          Console.Write("> ");
              string input = Console.ReadLine();
	          if (input == null || input.Length == 0)
	          {
		        continue;
	          }
	          input = input.Trim();
	          if (input.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
	          {
		        Environment.Exit(0);
	          }
	          apolloConfigDemo.GetConfig(input);
	        }
        }

        private void OnChanged(object sender, ConfigChangeEventArgs changeEvent)
        {
            Console.WriteLine("Changes for namespace {0}", changeEvent.Namespace);
            foreach (string key in changeEvent.ChangedKeys)
            {
                ConfigChange change = changeEvent.GetChange(key);
                Console.WriteLine("Change - key: {0}, oldValue: {1}, newValue: {2}, changeType: {3}",
                    change.PropertyName, change.OldValue, change.NewValue, change.ChangeType);
            }
        }

        private void PrintEnvInfo()
        {
            string message = string.Format("AppId: {0}, Env: {1}, DC: {2}, IP: {3}", Foundation.App.AppId, Foundation.Server.EnvType, Foundation.Server.DataCenter, Foundation.Net.HostAddress);
            Console.WriteLine(message);
        }
    }
}
