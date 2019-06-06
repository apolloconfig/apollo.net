using Com.Ctrip.Framework.Apollo;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Web;

namespace Apollo.AspNet.Demo
{
    public class Global : HttpApplication
    {
        public static IConfiguration Configuration { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            Configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddApollo(ConfigurationManager.AppSettings["Apollo.AppId"], ConfigurationManager.AppSettings["Apollo.MetaServer"]).AddNamespace("application.xml")
                .AddNamespace("application.json").AddDefault().Build();
        }
    }
}
