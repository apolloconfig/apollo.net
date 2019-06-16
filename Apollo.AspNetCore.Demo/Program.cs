using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Apollo.AspNetCore.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            YamlConfigAdapter.Register();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((cotnext, builder) => builder
                    .AddApollo(cotnext.Configuration.GetSection("apollo"))
                    .AddDefault(ConfigFileFormat.Xml)
                    .AddDefault(ConfigFileFormat.Json)
                    .AddDefault(ConfigFileFormat.Yml)
                    .AddDefault(ConfigFileFormat.Yaml)
                    .AddDefault())
                .UseStartup<Startup>();
    }
}
