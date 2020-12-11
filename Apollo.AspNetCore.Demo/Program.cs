using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Apollo.AspNetCore.Demo
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            YamlConfigAdapter.Register();

            return CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .AddApollo(false)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
    }
}
