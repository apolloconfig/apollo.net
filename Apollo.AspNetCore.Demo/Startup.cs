using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Apollo.AspNetCore.Demo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) { }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run((context) =>
            {
                context.Response.StatusCode = 404;

                var key = context.Request.Query["key"];
                if (string.IsNullOrWhiteSpace(key)) return Task.CompletedTask;

                var value = context.RequestServices.GetRequiredService<IConfiguration>()[key];
                if (string.IsNullOrWhiteSpace(value)) return Task.CompletedTask;

                context.Response.StatusCode = 200;
                context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

                return context.Response.WriteAsync(value);
            });
        }
    }
}
