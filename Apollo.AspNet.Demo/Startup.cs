using Apollo.AspNet.Demo;
using System;
using System.Configuration;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Startup))]
namespace Apollo.AspNet.Demo;

public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.Run(context =>
        {
            context.Response.StatusCode = 404;

            var key = context.Request.Query["key"];
            if (string.IsNullOrWhiteSpace(key)) return Task.CompletedTask;

            var con = ConfigurationManager.ConnectionStrings[key];
            if (con != null)
                return context.Response.WriteAsync($"{con.Name} {nameof(con.ConnectionString)}: {con.ConnectionString}, {nameof(con.ProviderName)}: {con.ProviderName}");

            var useLegend = DateTime.Now.Second % 2 == 1;
            var value = useLegend ? ConfigurationManager.AppSettings[key] : Global.Configuration[key];
            if (value != null) context.Response.StatusCode = 200;

            context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

            return context.Response.WriteAsync((useLegend ? "ConfigurationManager: " : "Configuration: ") + (value ?? "undefined"));
        });
    }
}