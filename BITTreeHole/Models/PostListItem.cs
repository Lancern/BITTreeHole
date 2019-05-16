using System;
using BITTreeHole.Data.Entities;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 为帖子列表项提供数据模型。
    /// </summary>
    public class PostListItem
    {
        /// <summary>
        /// 初始化 <see cref="PostListItem"/> 类的新实例。
        /// </summary>
        /// <param name="indexEntity">索引实体对象。</param>
        /// <param name="contentEntity">内容实体对象。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="indexEntity"/>为null
        /// </exception>
        public PostListItem(PostEntity indexEntity, PostContentEntity contentEntity = null)
        {
            if (indexEntity == null)
                throw new ArgumentNullException(nameof(indexEntity));

            Id = indexEntity.Id;
            RegionId = indexEntity.PostRegionId;
            Title = indexEntity.Title;
            CreationTime = indexEntity.CreationTime;
            UpdateTime = indexEntity.UpdateTime;
            Text = contentEntity?.Text ?? string.Empty;
            NumberOfVotes = indexEntity.NumberOfVotes;
            NumberOfComments = indexEntity.NumberOfComments;
        }
        
        /// <summary>
        /// 获取帖子 ID。
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; }
        
        /// <summary>
        /// 获取帖子板块 ID。
        /// </summary>
        [JsonProperty("regionId")]
        public int RegionId { get; }
        
        /// <summary>
        /// 获取帖子标题。
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; }
        
        /// <summary>
        /// 获取帖子的创建时间。
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; }
        
        /// <summary>
        /// 获取帖子的更新时间。
        /// </summary>
        [JsonProperty("updateTime")]
        public DateTime UpdateTime { get; }
        
        /// <summary>
        /// 获取帖子正文。
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; }
        
        /// <summary>
        /// 获取帖子的点赞数量。
        /// </summary>
        [JsonProperty("numberOfVotes")]
        public int NumberOfVotes { get; }
        
        /// <summary>
        /// 获取帖子的评论数量。
        /// </summary>
        [JsonProperty("numberOfComments")]
        public int NumberOfComments { get; }
    }
}
