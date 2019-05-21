using System;
using BITTreeHole.Utilities;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 封装帖子修改信息。
    /// </summary>
    public sealed class PostModificationInfo
    {
        /// <summary>
        /// 初始化 <see cref="PostModificationInfo"/> 类的新实例。
        /// </summary>
        [JsonConstructor]
        public PostModificationInfo()
        {
            Title = new Lazy<string>();
            Text = new Lazy<string>();
        }
        
        /// <summary>
        /// 获取帖子标题。
        /// </summary>
        [JsonConverter(typeof(LazyJsonConverter<string>))]
        [JsonProperty("title")]
        public Lazy<string> Title { get; private set; }
        
        /// <summary>
        /// 获取帖子内容。
        /// </summary>
        [JsonConverter(typeof(LazyJsonConverter<string>))]
        [JsonProperty("text")]
        public Lazy<string> Text { get; private set; }
    }
}
