using System;
using System.Threading.Tasks;
using BITTreeHole.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BITTreeHole.Services
{
    /// <summary>
    /// 提供微信API访问的依赖注入服务抽象。
    /// </summary>
    public interface IWechatApiService
    {
        /// <summary>
        /// 检查给定的微信授权访问代码（access_code）是否有效。
        /// </summary>
        /// <param name="wechatId">微信授权用户ID。</param>
        /// <param name="wechatAccessCode">微信授权访问代码（access_code）。</param>
        /// <returns>给定的微信授权访问代码是否有效。</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="wechatId"/>为null
        ///     或
        ///     <paramref name="wechatAccessCode"/>为null
        /// </exception>
        Task<bool> CheckAccessCodeValidity(string wechatId, string wechatAccessCode);
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
            /// <exception cref="ArgumentNullException">services为null。</exception>
            public static IServiceCollection AddDefaultWechatApiService(this IServiceCollection services)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));

                return services.AddSingleton<IWechatApiService, DefaultWechatApiService>();
            }
        }
    }
}
