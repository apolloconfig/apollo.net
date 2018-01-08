using Com.Ctrip.Framework.Apollo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Newtonsoft.Json;

namespace ApolloDemo
{
    class ConfigurationDemo
    {
        private static readonly IConfiguration Configuration;
        static ConfigurationDemo()
        {
            var builder = new ConfigurationBuilder();

            builder.AddJsonFile("appsettings.json");
                builder
                .AddApollo(builder.Build().GetSection("apollo"))
                .AddDefault()
                .AddtNamespace("TEST1.test");

            Configuration = builder.Build();
        }

        private readonly string DEFAULT_VALUE = "undefined";
        private readonly IConfiguration config;
        private readonly IConfiguration anotherConfig;

        public ConfigurationDemo()
        {
            config = Configuration;
            anotherConfig = Configuration.GetSection("TEST1.test");

            var services = new ServiceCollection();
            services.AddOptions()
                .Configure<Value>(config)
                .Configure<Value>("other", anotherConfig);

            var serviceProvider = services.BuildServiceProvider();

            var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<Value>>();

            optionsMonitor.OnChange(OnChanged);
        }

        public string GetConfig(string key)
        {
            var result = config.GetValue(key, DEFAULT_VALUE);
            if (result.Equals(DEFAULT_VALUE))
            {
                result = anotherConfig.GetValue(key, DEFAULT_VALUE);
            }
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Loading key: {0} with value: {1}", key, result);
            Console.ForegroundColor = color;

            return result;
        }

        private void OnChanged(Value value, string name)
        {
            Console.WriteLine(name + " has changed: " + JsonConvert.SerializeObject(value));
        }

        private class Value
        {
            public string timeout { get; set; }
        }
    }
}
