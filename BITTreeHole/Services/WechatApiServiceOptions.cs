namespace BITTreeHole.Services
{
    /// <summary>
    /// 为 <see cref="IWechatApiService"/> 的默认实现提供配置。
    /// </summary>
    public sealed class WechatApiServiceOptions
    {
        /// <summary>
        /// 用于访问微信 API 的 appid。
        /// </summary>
        public string AppId { get; set; }
        
        /// <summary>
        /// 用于访问微信 API 的 appsecret。
        /// </summary>
        public string AppSecret { get; set; }
    }
}
