using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 表示用户实体对象。
    /// </summary>
    public class UserEntity
    {
        /// <summary>
        /// 获取或设置用户ID。
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置用户微信ID。
        /// </summary>
        public string WechatId { get; set; }

        /// <summary>
        /// 获取或设置用户是否为管理员。
        /// </summary>
        public bool IsAdmin { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置用户发出的帖子集合。
        /// </summary>
        public virtual ICollection<PostEntity> UserPosts { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置用户的关注集合。
        /// </summary>
        public virtual ICollection<UserWatchPostEntity> Watches { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置用户的点赞集合。
        /// </summary>
        public virtual ICollection<UserVotePostEntity> Votes { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置用户的评论集合。
        /// </summary>
        public virtual ICollection<CommentEntity> Comments { get; set; }

        /// <summary>
        /// 配置用户实体对象的数据库模型。
        /// </summary>
        /// <param name="builder">用户实体对象的数据库模型构造器。</param>
        public static void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            // 配置主键为 ID 属性。
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.Id).ValueGeneratedOnAdd();

            // 配置 WechatID 属性上的 unique 索引。
            builder.HasIndex(entity => entity.WechatId)
                   .IsUnique();
            
            // 配置 IsAdmin 属性上的 unique 索引。
            builder.HasIndex(entity => entity.IsAdmin)
                   .IsUnique();
        }
    }
}
