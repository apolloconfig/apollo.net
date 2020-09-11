using Com.Ctrip.Framework.Apollo;
using Microsoft.Extensions.Configuration;
using System;

#if AspNetCoreHosting
using IHostBuilder = Microsoft.AspNetCore.Hosting.IWebHostBuilder;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostingBuilderExtensions
#else
namespace Microsoft.Extensions.Hosting
{
    public static class HostingBuilderExtensions
#endif
    {
        /// <param name="hostBuilder"></param>
        /// <param name="fromAppConfiguration">apollo配置源，false：环境变量、命令行之类；true：appsettings.json之类</param>
        /// <param name="key">apollo配置前缀</param>
        public static IHostBuilder AddApollo(this IHostBuilder hostBuilder, bool fromAppConfiguration = true, string key = "apollo") =>
            fromAppConfiguration
                ? hostBuilder.ConfigureAppConfiguration((_, builder) => builder.AddApollo(builder.Build().GetSection(key)))
                : hostBuilder.ConfigureAppConfiguration((context, builder) => builder.AddApollo(context.Configuration.GetSection(key)));

        public static IHostBuilder AddApollo(this IHostBuilder hostBuilder, string appId, string metaServer)
        {
            if (appId == null) throw new ArgumentNullException(nameof(appId));
            if (metaServer == null) throw new ArgumentNullException(nameof(metaServer));

            return hostBuilder.ConfigureAppConfiguration((_, builder) => builder.AddApollo(appId, metaServer));
        }

        public static IHostBuilder AddApollo(this IHostBuilder hostBuilder, Action<ApolloOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            var options = new ApolloOptions();

            configure(options);

            return hostBuilder.ConfigureAppConfiguration((_, builder) => builder.AddApollo(options));
        }
    }
}

