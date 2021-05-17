using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Apollo.AspNetCore.Demo
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.Run(context =>
            {
                context.Response.StatusCode = 404;

                var key = context.Request.Query["key"];
                if (string.IsNullOrWhiteSpace(key)) return Task.CompletedTask;

                var value = context.RequestServices.GetRequiredService<IConfiguration>()[key];
                if (value != null) context.Response.StatusCode = 200;

                context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

                return context.Response.WriteAsync(value ?? "undefined");
            });
        }
    }
}
