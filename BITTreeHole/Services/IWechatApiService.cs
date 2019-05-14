using System;
using System.Net.Http;
using System.Threading.Tasks;
using BITTreeHole.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BITTreeHole.Services
{
    /// <summary>
    /// 提供微信API访问的依赖注入服务抽象。
    /// </summary>
    public interface IWechatApiService
    {
        /// <summary>
        /// 使用指定的微信授权码获取微信访问代码（access_code）。
        /// </summary>
        /// <param name="wechatCode">用于获取 access_code 的微信授权码。</param>
        /// <returns>微信访问代码包装</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="WechatApiException"></exception>
        Task<WechatToken> GetWechatToken(string wechatCode);
    }
    
    namespace DependencyInjection
    {
        /// <summary>
        /// 为 <see cref="IWechatApiService"/> 提供依赖注入过程。
        /// </summary>
        internal static class DefaultWechatApiServiceExtensions
        {
            /// <summary>
            /// 将 <see cref="IWechatApiService"/> 依赖项的默认实现注入到给定的依赖服务集中。
            /// </summary>
            /// <param name="services">依赖服务集。</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">services 为 null。</exception>
            /// <exception cref="InvalidOperationException">用于访问微信 API 的 AppID 或 AppSecret 未配置。</exception>
            public static IServiceCollection AddDefaultWechatApiService(this IServiceCollection services,
                                                                        Action<WechatApiServiceOptions> options)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                if (options == null)
                    throw new ArgumentNullException(nameof(options));
                
                var optionObject = new WechatApiServiceOptions();
                options(optionObject);

                if (optionObject.AppId == null || optionObject.AppSecret == null)
                    throw new InvalidOperationException("用于访问微信 API 的 AppID 或 AppSecret 未配置。");

                return services.AddSingleton<IWechatApiService, DefaultWechatApiService>(
                    serviceProvider => new DefaultWechatApiService(
                        optionObject,
                        serviceProvider.GetService<IHttpClientFactory>(),
                        serviceProvider.GetService<ILogger<DefaultWechatApiService>>()));
            }
        }
    }
}
