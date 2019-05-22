using System.ComponentModel.DataAnnotations;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 封装帖子创建信息。
    /// </summary>
    public sealed class PostCreationInfo
    {
        /// <summary>
        /// 获取帖子所属板块 ID。
        /// </summary>
        public int RegionId { get; private set; }
        
        /// <summary>
        /// 获取帖子标题。
        /// </summary>
        [Required]
        public string Title { get; private set; }
        
        // TODO: 为 Title 字段添加 MaxLength 约束
        
        /// <summary>
        /// 获取帖子正文。
        /// </summary>
        [Required]
        [MaxLength(2048)]
        public string Text { get; private set; }
    }
}
