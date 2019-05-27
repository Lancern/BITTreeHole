using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 封装帖子创建信息。
    /// </summary>
    public sealed class RegionCreationInfo
    {
        /// <summary>
        /// 获取或设置帖子图像的 Base64 编码。
        /// </summary>
        [JsonProperty("imageBase64")]
        public string ImageBase64 { get; private set; }
    }
}
