using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Apollo.Configuration.Demo
{
    internal class ConfigurationDemo
    {
        private static readonly IConfiguration Configuration;
        static ConfigurationDemo()
        {
            var builder = new ConfigurationBuilder();

            builder.AddEnvironmentVariables();

            var apollo = builder.Build().GetSection("apollo").Get<ApolloOptions>();

            //apollo.HttpMessageHandlerFactory = () => new HttpClientHandler
            //{
            //    UseProxy = true,
            //    Proxy = new WebProxy(new Uri("http://127.0.0.1:8888"))
            //};

            builder.AddApollo(builder.Build().GetSection("apollo"))
                .AddDefault(ConfigFileFormat.Xml)
                .AddDefault(ConfigFileFormat.Json)
                .AddDefault(ConfigFileFormat.Yml)
                .AddDefault(ConfigFileFormat.Yaml)
                .AddDefault();

            Configuration = builder.Build();
        }

        private readonly string DEFAULT_VALUE = "undefined";
        private readonly IConfiguration config;
        private readonly IConfiguration anotherConfig;

        public ConfigurationDemo()
        {
            config = Configuration;
            anotherConfig = Configuration.GetSection("a");

            var services = new ServiceCollection();
            services.AddOptions()
                .Configure<Value>(config)
                .Configure<Value>("other", anotherConfig);
#pragma warning disable 618
            services.AddSingleton<ApolloConfigurationManager>();
#pragma warning restore 618
            var serviceProvider = services.BuildServiceProvider();

            var optionsMonitor = serviceProvider.GetService<IOptionsMonitor<Value>>();

            optionsMonitor.OnChange(OnChanged);

            //new ConfigurationManagerDemo(serviceProvider.GetService<ApolloConfigurationManager>());
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
            public string Timeout { get; set; } = "";
        }
    }
}
