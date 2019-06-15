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
                context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

                var key = context.Request.Query["key"];
                return string.IsNullOrWhiteSpace(key)
                    ? Task.CompletedTask
                    : context.Response.WriteAsync(context.RequestServices.GetRequiredService<IConfiguration>()[key]);
            });
        }
    }
}
