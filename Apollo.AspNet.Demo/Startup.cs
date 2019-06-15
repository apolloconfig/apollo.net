using Apollo.AspNet.Demo;
using Microsoft.Owin;
using Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Startup))]
namespace Apollo.AspNet.Demo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Run(context =>
            {
                context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

                var key = context.Request.Query["key"];
                if (string.IsNullOrWhiteSpace(key))
                    return Task.CompletedTask;

                var value = DateTime.Now.Second % 2 == 1 ? "ConfigurationManager: " + ConfigurationManager.AppSettings[key] : "Configuration: " + Global.Configuration[key];
                return value == null ? Task.CompletedTask : context.Response.WriteAsync(value);
            });
        }
    }
}
