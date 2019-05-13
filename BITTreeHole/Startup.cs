using System;
using BITTreeHole.Data.DependencyInjection;
using BITTreeHole.Services.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BITTreeHole
{
    public class Startup
    {
        public Startup(ILogger<Startup> logger, 
                       IConfiguration configuration, 
                       IHostingEnvironment hostingEnvironment)
        {
            Logger = logger;
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        
        public ILogger<Startup> Logger { get; }
        
        public IHostingEnvironment HostingEnvironment { get; }

        private void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddDefaultWechatApiService();
            
            // 加载 JWT 相关配置
            var jwtCertFileName = Configuration.GetSection("JWT")
                                               .GetValue<string>("CertFile", null);
            if (jwtCertFileName == null)
            {
                if (HostingEnvironment.IsDevelopment())
                {
                    Logger.LogWarning("未配置JWT服务的加密方式。JWT服务将降级到无加密方式。");
                }
                else
                {
                    Logger.LogCritical("未配置JWT服务的加密方式。");
                    
                    // 抛出异常以强制使应用程序非正常退出。
                    throw new Exception("未配置JWT服务的加密方式。");
                }
            }

            services.AddDefaultDataFacade(
                Configuration.GetConnectionString("mysql"),
                Configuration.GetConnectionString("mongodb"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ConfigureApplicationServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
