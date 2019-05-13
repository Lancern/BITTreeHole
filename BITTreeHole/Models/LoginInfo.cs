using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 为用户登录提供数据模型。
    /// </summary>
    public sealed class LoginInfo
    {
        /// <summary>
        /// 初始化 <see cref="LoginInfo"/> 类的新实例。
        /// </summary>
        public LoginInfo()
        {
            WechatId = null;
            WechatAccessCode = null;
        }
        
        /// <summary>
        /// 获取或设置授权用户的微信ID。
        /// </summary>
        [Required]
        [JsonProperty("wechatId")]
        public string WechatId { get; set; }
        
        /// <summary>
        /// 获取或设置客户端得到的微信访问代码（access_code）。
        /// </summary>
        [Required]
        [JsonProperty("wechatAccessCode")]
        public string WechatAccessCode { get; set; }
    }
}
