using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace BITTreeHole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config")
                                    .GetCurrentClassLogger();

            try
            {
                logger.Info("启动 BITTreeHole 服务端 Web API 服务");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Web API 服务因未处理的异常而终止：{0}：{1}", ex.GetType(), ex.Message);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
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
