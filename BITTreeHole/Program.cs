using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace BITTreeHole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration((host, config) =>
                   {
                       config.SetBasePath(Directory.GetCurrentDirectory());
                       config.AddJsonFile("AppCriticalSettings.json", optional: true, reloadOnChange: false);
                   })
                   .ConfigureLogging(builder =>
                   {
                       builder.ClearProviders();
                       builder.SetMinimumLevel(LogLevel.Trace);
                   })
                   .UseNLog()
                   .UseStartup<Startup>();
    }
}
