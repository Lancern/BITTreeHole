using System;
using BITTreeHole.Data.DependencyInjection;
using BITTreeHole.Services;
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
        private const string CorsPolicyName = "BITTreeHoleCors";
    
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
            // 加载微信 API 相关配置
//            var wechatAppId = Configuration.GetSection("Wechat")
//                                           .GetValue<string>("AppId", null);
//            var wechatAppSecret = Configuration.GetSection("Wechat")
//                                               .GetValue<string>("AppSecret", null);
//            if (string.IsNullOrEmpty(wechatAppId) || string.IsNullOrEmpty(wechatAppSecret))
//            {
//                if (HostingEnvironment.IsDevelopment())
//                {
//                    Logger.LogWarning("未配置访问微信 API 所需的 AppId 以及 AppSecret。服务将降级为基于 mock 数据的实现。");
//                    services.AddMockWechatApiService();
//                }
//                else
//                {
//                    Logger.LogCritical("未配置访问微信 API 所需的 AppId 以及 AppSecret。");
//                    throw new Exception("未配置访问微信 API 所需的 AppId 以及 AppSecret。");
//                }
//            }
//            else
//            {
//                services.AddDefaultWechatApiService(options =>
//                {
//                    options.AppId = wechatAppId;
//                    options.AppSecret = wechatAppSecret;
//                });
//            }

            services.AddIdentityWechatApiService();
            
            // 加载 JWT 相关配置
            var jwtCertFileName = Configuration.GetSection("JWT")
                                               .GetValue<string>("CertFile", null);
            if (string.IsNullOrEmpty(jwtCertFileName))
            {
                if (HostingEnvironment.IsDevelopment())
                {
                    Logger.LogWarning("未配置JWT服务的加密方式。JWT服务将降级到无加密方式。");
                    services.AddJoseJwtService();
                }
                else
                {
                    Logger.LogCritical("未配置JWT服务的加密方式。");
                    throw new Exception("未配置JWT服务的加密方式。");
                }
            }
            else
            {
                services.AddJoseJwtService(options => options.UseRSA256(jwtCertFileName));
            }

            services.AddDefaultDataFacade(
                Configuration.GetConnectionString("mysql"),
                Configuration.GetConnectionString("mongodb"));

            services.AddDefaultEntityFactory();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            // 配置 CORS
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddHttpClient();

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

            // 启用 CORS 处理
            app.UseCors(CorsPolicyName);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
