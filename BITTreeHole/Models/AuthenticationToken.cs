using BITTreeHole.Services;
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
            WechatToken = null;
        }
        
        /// <summary>
        /// 初始化 <see cref="AuthenticationToken"/> 类的新实例。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="wechatToken">用于与微信 API 交互的 Token。</param>
        public AuthenticationToken(int userId, WechatToken wechatToken)
        {
            UserId = userId;
            WechatToken = wechatToken;
        }
        
        /// <summary>
        /// 获取用户 ID。
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; private set; }
        
        /// <summary>
        /// 获取用于与微信 API 交互的 Token。
        /// </summary>
        [JsonProperty("wechatToken")]
        public WechatToken WechatToken { get; private set; }
    }
}
