using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 为评论创建提供数据模型。
    /// </summary>
    public sealed class CommentCreationInfo
    {
        /// <summary>
        /// 初始化 <see cref="CommentCreationInfo"/> 类的新实例。
        /// </summary>
        [JsonConstructor]
        public CommentCreationInfo()
        {
            Text = null;
        }

        /// <summary>
        /// 获取评论正文。
        /// </summary>
        [JsonProperty("text")]
        [Required]
        public string Text { get; private set; }
    }
}
