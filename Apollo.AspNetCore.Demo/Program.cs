using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Enums;
using Com.Ctrip.Framework.Apollo.Logging;
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

            LogManager.UseConsoleLogging(LogLevel.Debug);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               //.ConfigureAppConfiguration((context, builder) => builder //使用环境变量、命令行之类，建议Docker中运行使用此方式
               //     .AddApollo(context.Configuration.GetSection("apollo"))
               .ConfigureAppConfiguration(builder => builder //普通方式，一般配置在appsettings.json中
                   .AddApollo(builder.Build().GetSection("apollo"))
                    //.AddDefault(ConfigFileFormat.Xml)
                    //.AddDefault(ConfigFileFormat.Json)
                    //.AddDefault(ConfigFileFormat.Yml)
                    //.AddDefault(ConfigFileFormat.Yaml)
                    .AddDefault())
                .UseStartup<Startup>();
    }
}
