using System;
using System.Security.Cryptography.X509Certificates;
using BITTreeHole.Services.Implementations;
using Jose;

namespace BITTreeHole.Services
{
    /// <summary>
    /// 封装 <see cref="JoseJwtService"/> 服务实现的参数。
    /// </summary>
    public sealed class JoseJwtServiceOptions
    {
        /// <summary>
        /// 初始化 <see cref="JoseJwtServiceOptions"/> 类的新实例。
        /// </summary>
        public JoseJwtServiceOptions()
        {
            EncryptKey = null;
            DecryptKey = null;
            Algorithm = JwsAlgorithm.none;
        }
        
        /// <summary>
        /// 获取或设置用于加密的密钥。
        /// </summary>
        public object EncryptKey { get; set; }
        
        /// <summary>
        /// 获取或设置用于解密的密钥。
        /// </summary>
        public object DecryptKey { get; set; }
        
        /// <summary>
        /// 获取或设置加密/解密算法。
        /// </summary>
        public JwsAlgorithm Algorithm { get; set; }
    }

    /// <summary>
    /// 为 <see cref="JoseJwtServiceOptions"/> 提供扩展方法。
    /// </summary>
    public static class JoseJwtServiceOptionsExtension
    {
        /// <summary>
        /// 配置 JWT 服务以使用 RSA256 加密方式。
        /// </summary>
        /// <param name="options"></param>
        /// <param name="certificateFileName">包含密钥的数字签名文件。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options"/>为null
        ///     或
        ///     <paramref name="certificateFileName"/>为null
        /// </exception>
        public static JoseJwtServiceOptions UseRSA256(this JoseJwtServiceOptions options,
                                                      string certificateFileName)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (certificateFileName == null)
                throw new ArgumentNullException(nameof(certificateFileName));

            using (var certificate = new X509Certificate2(certificateFileName))
            {
                options.EncryptKey = certificate.GetRSAPrivateKey();
                options.DecryptKey = certificate.GetRSAPublicKey();
            }

            options.Algorithm = JwsAlgorithm.RS256;
            return options;
        }
    }
}
