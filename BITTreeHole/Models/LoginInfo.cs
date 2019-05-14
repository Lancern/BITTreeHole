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
        /// 获取或设置微信授权码。
        /// </summary>
        [JsonProperty("code")]
        public string WechatCode { get; private set; }
    }
}
