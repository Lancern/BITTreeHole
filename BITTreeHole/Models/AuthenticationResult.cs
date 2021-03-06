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
        [JsonProperty("succeed")]
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// 获取身份验证过程产生的消息。
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; private set; }
        
        /// <summary>
        /// 获取用户 ID。
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; private set; }

        /// <summary>
        /// 获取用户是否为管理员。
        /// </summary>
        [JsonProperty("admin")]
        public bool IsAdmin { get; private set; }

        /// <summary>
        /// 获取用户身份标识JWT。
        /// </summary>
        [JsonProperty("jwt")]
        public string Jwt { get; private set; }

        /// <summary>
        /// 创建表示身份验证成功的 <see cref="AuthenticationResult"/> 对象。
        /// </summary>
        /// <param name="userId">用户 ID</param>
        /// <param name="isAdmin">用户是否为管理员</param>
        /// <param name="jwt">包含用户身份标识信息的 JWT。</param>
        /// <param name="message">身份验证过程中产生的消息。</param>
        /// <returns></returns>
        public static AuthenticationResult Success(int userId, bool isAdmin, string jwt, string message = null)
        {
            return new AuthenticationResult
            {
                IsSuccessful = true,
                Message = message,
                UserId = userId,
                IsAdmin = isAdmin,
                Jwt = jwt
            };
        }

        /// <summary>
        /// 创建表示身份验证失败的 <see cref="AuthenticationResult"/> 对象。
        /// </summary>
        /// <param name="message">身份验证过程中产生的消息。</param>
        /// <returns></returns>
        public static AuthenticationResult Failure(string message = null)
        {
            return new AuthenticationResult
            {
                IsSuccessful = false,
                Message = message
            };
        }
    }
}
