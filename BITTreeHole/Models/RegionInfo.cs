using System;
using BITTreeHole.Data.Entities;
using Newtonsoft.Json;

namespace BITTreeHole.Models
{
    /// <summary>
    /// 为帖子板块信息提供数据模型。
    /// </summary>
    public sealed class RegionInfo
    {
        /// <summary>
        /// 从给定的 <see cref="PostRegionEntity"/> 对象初始化 <see cref="RegionInfo"/> 对象的新实例。
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="ArgumentNullException"><paramref name="entity"/>为null</exception>
        public RegionInfo(PostRegionEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Id = entity.Id;
            Name = entity.Title;
        }
        
        /// <summary>
        /// 获取板块 ID。
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; }
        
        /// <summary>
        /// 获取板块名称。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }
    }
}
