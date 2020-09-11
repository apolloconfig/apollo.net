using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace Apollo.AspNetCore.Demo
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            YamlConfigAdapter.Register();

            return CreateWebHostBuilder(args).Build().RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder<Startup>(args).AddApollo();
    }
}
