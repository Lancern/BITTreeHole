using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 表示帖子实体对象。
    /// </summary>
    public class PostEntity
    {
        /// <summary>
        /// 获取或设置帖子的 ID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置帖子的作者 ID。
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// 获取或设置帖子的标题。
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 获取或设置帖子的创建时间。
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 获取或设置帖子的最后修改时间。
        /// </summary>
        public DateTime UpdateTime { get; set; }
        
        /// <summary>
        /// 获取或设置帖子获得的点赞数量。
        /// </summary>
        public int NumberOfVotes { get; set; }
        
        /// <summary>
        /// 获取或设置帖子获得的评论数量。
        /// </summary>
        public int NumberOfComments { get; set; }
        
        /// <summary>
        /// 获取或设置帖子正文实体对象在 MongoDB 中的 Key。
        /// </summary>
        public byte[] ContentId { get; set; }
        
        /// <summary>
        /// 帖子是否已经被删除。
        /// </summary>
        public bool IsRemoved { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置帖子的作者。
        /// </summary>
        public virtual UserEntity Author { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置帖子的关注者。
        /// </summary>
        public virtual ICollection<UserWatchPostEntity> Watchers { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置帖子的点赞者。
        /// </summary>
        public virtual ICollection<UserVotePostEntity> Voters { get; set; }

        /// <summary>
        /// 配置 <see cref="PostEntity"/> 实体对象的数据库模型。
        /// </summary>
        /// <param name="builder"></param>
        public static void Configure(EntityTypeBuilder<PostEntity> builder)
        {
            // 配置主键
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.Id)
                   .ValueGeneratedOnAdd();

            // 配置"用户发表帖子"一对多关系
            builder.HasOne(entity => entity.Author)
                   .WithMany(entity => entity.UserPosts)
                   .HasForeignKey(entity => entity.AuthorId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 配置在 IsRemoved 字段上的索引
            builder.HasIndex(entity => entity.IsRemoved);
        }
    }
}
