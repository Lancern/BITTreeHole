using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 为用户关注帖子这一多对多关系提供实体对象。
    /// </summary>
    public class UserWatchPostEntity
    {
        /// <summary>
        /// 获取或设置用户 ID。
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 获取或设置帖子 ID。
        /// </summary>
        public int PostId { get; set; }
        
        /// <summary>
        /// 获取或设置关注的创建时间。
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 获取或设置用户上一次拉取被关注的帖子信息的时间。
        /// </summary>
        public DateTime LastViewTime { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置用户实体对象。
        /// </summary>
        public virtual UserEntity User { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置帖子实体对象。
        /// </summary>
        public virtual PostEntity Post { get; set; }

        /// <summary>
        /// 配置 <see cref="UserWatchPostEntity"/> 的数据库模型。
        /// </summary>
        /// <param name="builder"></param>
        public static void Configure(EntityTypeBuilder<UserWatchPostEntity> builder)
        {
            // 配置复合主键
            builder.HasKey(nameof(UserId), nameof(PostId));
            
            // 分别配置 UserId, PostId 字段上的索引
            builder.HasIndex(entity => entity.UserId);
            builder.HasIndex(entity => entity.PostId);

            // 配置与用户的一对多关系
            builder.HasOne(entity => entity.User)
                   .WithMany(entity => entity.Watches)
                   .HasForeignKey(entity => entity.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 配置与帖子的一对多关系
            builder.HasOne(entity => entity.Post)
                   .WithMany(entity => entity.Watchers)
                   .HasForeignKey(entity => entity.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
