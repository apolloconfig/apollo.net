using Microsoft.Extensions.Configuration;
using System;
using Com.Ctrip.Framework.Apollo;

namespace ApolloDemo
{
    class ConfigurationDemo
    {
        private static readonly IConfiguration Configuration;
        static ConfigurationDemo()
        {
            var builder = new ConfigurationBuilder();

            builder
                .AddJsonFile("appsettings.json")
                .AddApollo()
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
    }
}
