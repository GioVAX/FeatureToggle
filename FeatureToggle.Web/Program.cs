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
                .AddCommandLine(args)
                .Build();

            BuildWebHost(configuration)
                .Run();
        }

        public static IWebHost BuildWebHost(IConfiguration configuration)
        {
            var webHost = WebHost.CreateDefaultBuilder()
                .UseConfiguration(configuration);

            var url = configuration["server.urls"];
            if (!string.IsNullOrWhiteSpace(url))
                webHost = webHost.UseUrls(url);

            return webHost.UseStartup<Startup>()
                .Build();
        }
    }
}
