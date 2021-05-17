using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Web;

namespace Apollo.AspNet.Demo
{
    public class Global : HttpApplication
    {
        public static IConfiguration Configuration { get; private set; } = default!;

        protected void Application_Start(object sender, EventArgs e)
        {
            YamlConfigAdapter.Register();

            Configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddApollo(new ApolloOptions
                {
                    AppId = ConfigurationManager.AppSettings["Apollo:AppId"],
                    MetaServer = ConfigurationManager.AppSettings["Apollo:MetaServer"],
                    Secret = ConfigurationManager.AppSettings["Apollo:Secret"]
                })
                .AddDefault(ConfigFileFormat.Xml)
                .AddDefault(ConfigFileFormat.Json)
                .AddDefault(ConfigFileFormat.Yml)
                .AddDefault(ConfigFileFormat.Yaml)
                .AddDefault()
                .Build();
        }
    }
}
