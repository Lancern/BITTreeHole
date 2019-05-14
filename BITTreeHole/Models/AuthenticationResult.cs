using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 为用户身份验证结果提供数据模型。
    /// </summary>
    public sealed class AuthenticationResult
    {
        /// <summary>
        /// 用户身份验证是否成功。
        /// </summary>
        [JsonProperty("succeeded")]
        public bool IsSuccessful { get; private set; }
        
        /// <summary>
        /// 获取或设置用户身份标识JWT。
        /// </summary>
        [JsonProperty("jwt")]
        public string Jwt { get; private set; }

        /// <summary>
        /// 创建表示身份验证成功的 <see cref="AuthenticationResult"/> 对象。
        /// </summary>
        /// <param name="jwt">包含用户身份标识信息的 JWT。</param>
        /// <returns></returns>
        public static AuthenticationResult Success(string jwt)
        {
            return new AuthenticationResult
            {
                IsSuccessful = true,
                Jwt = jwt
            };
        }

        /// <summary>
        /// 创建表示身份验证失败的 <see cref="AuthenticationResult"/> 对象。
        /// </summary>
        /// <returns></returns>
        public static AuthenticationResult Failure()
        {
            return new AuthenticationResult
            {
                IsSuccessful = false
            };
        }
    }
}
