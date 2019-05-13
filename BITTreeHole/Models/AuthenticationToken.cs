using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 提供用户身份标识。
    /// </summary>
    public sealed class AuthenticationToken
    {
        /// <summary>
        /// 初始化 <see cref="AuthenticationToken"/> 类的新实例。
        /// </summary>
        [JsonConstructor]
        public AuthenticationToken()
        {
            UserId = 0;
        }
        
        /// <summary>
        /// 初始化 <see cref="AuthenticationToken"/> 类的新实例。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        public AuthenticationToken(int userId)
        {
            UserId = userId;
        }
        
        /// <summary>
        /// 用户 ID。
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; private set; }
    }
}
