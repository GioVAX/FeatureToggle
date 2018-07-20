using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FeatureToggle.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .Build();

            BuildWebHost(args, configuration)
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args, IConfiguration configuration)
        {
            var webHost = WebHost.CreateDefaultBuilder(args);

            var url = configuration["server.urls"];
            if (!string.IsNullOrWhiteSpace(url))
                webHost.UseUrls(url);

            return webHost.UseStartup<Startup>()
                .Build();
        }
    }
}
