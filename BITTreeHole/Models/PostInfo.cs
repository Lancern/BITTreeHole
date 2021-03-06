using System;
using BITTreeHole.Data.Entities;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 为帖子详细信息提供数据模型。
    /// </summary>
    public sealed class PostInfo
    {
        /// <summary>
        /// 初始化 <see cref="PostInfo"/> 类的新实例。
        /// </summary>
        /// <param name="indexEntity">索引实体对象。</param>
        /// <param name="contentEntity">内容实体对象。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="indexEntity"/>为null
        ///     或
        ///     <paramref name="contentEntity"/>为null
        /// </exception>
        public PostInfo(PostEntity indexEntity, PostContentEntity contentEntity, bool isVoted)
        {
            if (indexEntity == null)
                throw new ArgumentNullException(nameof(indexEntity));
            if (contentEntity == null)
                throw new ArgumentNullException(nameof(contentEntity));

            Title = indexEntity.Title;
            Text = contentEntity.Text;
            AuthorId = indexEntity.AuthorId;
            CreationTime = indexEntity.CreationTime;
            UpdateTime = indexEntity.UpdateTime;
            NumberOfImages = contentEntity.ImageIds.Length;
            NumberOfVotes = indexEntity.NumberOfVotes;
            NumberOfComments = indexEntity.NumberOfComments;
            IsVoted = isVoted;
        }
        
        /// <summary>
        /// 获取帖子标题。
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; }
        
        /// <summary>
        /// 获取帖子正文。
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; }
        
        /// <summary>
        /// 获取帖子作者。
        /// </summary>
        [JsonProperty("authorId")]
        public int AuthorId { get; }
        
        /// <summary>
        /// 获取帖子创建时间。
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; }
        
        /// <summary>
        /// 获取帖子更新时间。
        /// </summary>
        [JsonProperty("updateTime")]
        public DateTime UpdateTime { get; }
        
        /// <summary>
        /// 获取贴子的图片数量。
        /// </summary>
        [JsonProperty("numberOfImages")]
        public int NumberOfImages { get; }
        
        /// <summary>
        /// 获取点赞数量。
        /// </summary>
        [JsonProperty("numberOfVotes")]
        public int NumberOfVotes { get; }
        
        /// <summary>
        /// 获取评论数量。
        /// </summary>
        [JsonProperty("numberOfComments")]
        public int NumberOfComments { get; }
        
        /// <summary>
        /// 获取当前用户是否点赞了当前帖子。
        /// </summary>
        [JsonProperty("isVoted")]
        public bool IsVoted { get; }
    }
}
