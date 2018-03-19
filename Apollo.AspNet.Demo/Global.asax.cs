using Com.Ctrip.Framework.Apollo;
using Microsoft.Extensions.Configuration;
using System;
using System.Web;

namespace Apollo.AspNet.Demo
{
    public class Global : HttpApplication
    {
        public static IConfiguration Configuration { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            var builder = new ConfigurationBuilder().AddJsonFile(@"App_Data\appsettings.json");

            builder.AddApollo(builder.Build().GetSection("apollo")).AddNamespace("TEST1.test").AddDefault();

            Configuration = builder.Build();
        }
    }
}