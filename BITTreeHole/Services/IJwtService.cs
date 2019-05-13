using System;
using BITTreeHole.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BITTreeHole.Services
{
    /// <summary>
    /// 提供 JWT 服务的依赖注入抽象。
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// 将给定的对象序列化为 JWT。
        /// </summary>
        /// <param name="o">要序列化的对象。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="o"/>为null。</exception>
        string Encode(object o);

        /// <summary>
        /// 从给定的 JWT 反序列化对象实例。
        /// </summary>
        /// <param name="jwt">包含对象实例的 JWT 序列。</param>
        /// <typeparam name="T">需要反序列化的对象的类型。</typeparam>
        /// <returns>反序列化出的对象。</returns>
        T Decode<T>(string jwt);
    }

    namespace DependencyInjection
    {
        /// <summary>
        /// 为 <see cref="IJwtService"/> 提供依赖注入过程。
        /// </summary>
        public static class JwtServiceExtensions
        {
            /// <summary>
            /// 将基于 jose-jwt 实现的 <see cref="IJwtService"/> 服务注入到给定的依赖服务集中。
            /// </summary>
            /// <param name="services"></param>
            /// <param name="options">为服务提供参数。</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">
            ///     <paramref name="services"/>为null
            ///     或
            ///     <paramref name="options"/>为null
            /// </exception>
            public static IServiceCollection AddJoseJwtService(this IServiceCollection services,
                                                               Action<JoseJwtServiceOptions> options = null)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                
                var optionObject = new JoseJwtServiceOptions();
                options?.Invoke(optionObject);

                return services.AddSingleton<IJwtService, JoseJwtService>(_ => new JoseJwtService(optionObject));
            }
        }
    }
}
