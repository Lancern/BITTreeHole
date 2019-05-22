using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 为"用户点赞帖子"这一多对多关系提供实体对象。
    /// </summary>
    public class UserVotePostEntity
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
        /// 获取或设置点赞时间。
        /// </summary>
        public DateTime CreationTime { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置点赞用户。
        /// </summary>
        public virtual UserEntity User { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置被点赞的帖子。
        /// </summary>
        public virtual PostEntity Post { get; set; }

        /// <summary>
        /// 创建一个新的 <see cref="UserVotePostEntity"/> 对象。
        /// </summary>
        /// <param name="userId">用户 ID。</param>
        /// <param name="postId">帖子 ID。</param>
        /// <returns>创建的 <see cref="UserVotePostEntity"/> 对象。</returns>
        public static UserVotePostEntity Create(int userId, int postId)
        {
            return new UserVotePostEntity
            {
                UserId = userId,
                PostId = postId,
                CreationTime = DateTime.Now
            };
        }

        /// <summary>
        /// 配置 <see cref="UserVotePostEntity"/> 的数据库模型。
        /// </summary>
        /// <param name="builder"></param>
        public static void Configure(EntityTypeBuilder<UserVotePostEntity> builder)
        {
            // 配置复合主键
            builder.HasKey(nameof(UserId), nameof(PostId));

            // 分别配置 UserId 以及 PostId 字段上的索引
            builder.HasIndex(entity => entity.UserId);
            builder.HasIndex(entity => entity.PostId);

            // 配置与用户的一对多关系
            builder.HasOne(entity => entity.User)
                   .WithMany(entity => entity.Votes)
                   .HasForeignKey(entity => entity.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 配置与帖子的一对多关系
            builder.HasOne(entity => entity.Post)
                   .WithMany(entity => entity.Voters)
                   .HasForeignKey(entity => entity.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
