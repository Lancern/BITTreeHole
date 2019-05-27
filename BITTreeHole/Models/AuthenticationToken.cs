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
            IsAdmin = false;
            WechatToken = null;
        }
        
        /// <summary>
        /// 初始化 <see cref="AuthenticationToken"/> 类的新实例。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="isAdmin">用户是否拥有管理员权限。</param>
        /// <param name="wechatToken">用于与微信 API 交互的 Token。</param>
        public AuthenticationToken(int userId, bool isAdmin, WechatToken wechatToken)
        {
            UserId = userId;
            IsAdmin = isAdmin;
            WechatToken = wechatToken;
        }
        
        /// <summary>
        /// 获取用户 ID。
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; private set; }
        
        /// <summary>
        /// 获取用户是否拥有管理员权限。
        /// </summary>
        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; private set; }
        
        /// <summary>
        /// 获取用于与微信 API 交互的 Token。
        /// </summary>
        [JsonProperty("wechatToken")]
        public WechatToken WechatToken { get; private set; }
    }
}
