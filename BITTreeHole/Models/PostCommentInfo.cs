using System;
using System.Collections.Generic;
using BITTreeHole.Data.Entities;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 封装帖子评论信息。
    /// </summary>
    public sealed class PostCommentInfo
    {
        /// <summary>
        /// 初始化 <see cref="PostCommentInfo"/> 类的新实例。
        /// </summary>
        /// <param name="indexEntity">索引实体对象。</param>
        /// <param name="contentEntity">内容实体对象。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="indexEntity"/>为null
        ///     或
        ///     <paramref name="contentEntity"/>为null
        /// </exception>
        public PostCommentInfo(CommentEntity indexEntity, CommentContentEntity contentEntity)
        {
            if (indexEntity == null)
                throw new ArgumentNullException(nameof(indexEntity));
            if (contentEntity == null)
                throw new ArgumentNullException(nameof(contentEntity));

            Id = indexEntity.Id;
            AuthorId = indexEntity.AuthorId;
            CreationTime = indexEntity.CreationTime;
            Text = contentEntity.Text;
            Comments = new List<PostCommentInfo>();
        }
        
        /// <summary>
        /// 获取评论 ID。
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; private set; }
        
        /// <summary>
        /// 获取评论作者 ID。
        /// </summary>
        [JsonProperty("authorId")]
        public int AuthorId { get; private set; }
        
        /// <summary>
        /// 获取评论创建时间。
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; private set; }
        
        /// <summary>
        /// 获取评论内容。
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; private set; }
        
        /// <summary>
        /// 获取子评论列表。
        /// </summary>
        [JsonProperty("comments")]
        public List<PostCommentInfo> Comments { get; private set; }
    }
}
