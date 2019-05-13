using System;
using Jose;

namespace BITTreeHole.Services.Implementations
{
    /// <summary>
    /// 为 <see cref="IJwtService"/> 提供基于 jose-jwt 的实现。
    /// </summary>
    internal sealed class JoseJwtService : IJwtService
    {
        private readonly JoseJwtServiceOptions _options;

        /// <summary>
        /// 初始化 <see cref="JoseJwtService"/> 类的新实例。
        /// </summary>
        /// <param name="options">服务选项。</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JoseJwtService(JoseJwtServiceOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        
        /// <inheritdoc />
        public string Encode(object o)
        {
            return JWT.Encode(o, _options.EncryptKey, _options.Algorithm);
        }

        /// <inheritdoc />
        public T Decode<T>(string jwt)
        {
            return JWT.Decode<T>(jwt, _options.DecryptKey, _options.Algorithm);
        }
    }
}
