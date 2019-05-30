using System;
using BITTreeHole.Data.Entities;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 封装用户发表的评论信息。
    /// </summary>
    public class UserCommentInfo
    {
        /// <summary>
        /// 初始化 <see cref="UserCommentInfo"/> 类的新实例。
        /// </summary>
        /// <param name="indexEntity">评论索引实体对象。</param>
        /// <param name="contentEntity">评论内容实体对象。</param>
        /// <param name="postId">评论所属帖子 ID。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="indexEntity"/>为null
        ///     或
        ///     <paramref name="contentEntity"/>为null
        ///     或
        ///     <paramref name="postEntity"/>为null
        /// </exception>
        public UserCommentInfo(CommentEntity indexEntity, CommentContentEntity contentEntity, PostEntity postEntity)
        {
            if (indexEntity == null)
                throw new ArgumentNullException(nameof(indexEntity));
            if (contentEntity == null)
                throw new ArgumentNullException(nameof(contentEntity));
            if (postEntity == null)
                throw new ArgumentNullException(nameof(postEntity));
            
            Id = indexEntity.Id;
            AuthorId = indexEntity.AuthorId;
            PostId = postEntity.Id;
            PostTitle = postEntity.Title;
            CreationTime = indexEntity.CreationTime;
            Text = contentEntity.Text;
        }
        
        /// <summary>
        /// 获取评论 ID。
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; }
        
        /// <summary>
        /// 获取作者 ID。
        /// </summary>
        [JsonProperty("authorId")]
        public int AuthorId { get; }
        
        /// <summary>
        /// 获取帖子 ID。
        /// </summary>
        [JsonProperty("postId")]
        public int PostId { get; }
        
        /// <summary>
        /// 获取帖子标题。
        /// </summary>
        [JsonProperty("postTitle")]
        public string PostTitle { get; }
        
        /// <summary>
        /// 获取评论创建时间。
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; }
        
        /// <summary>
        /// 获取评论内容。
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; }
    }
}
