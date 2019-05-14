using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 表示评论实体对象。
    /// </summary>
    public class CommentEntity
    {
        /// <summary>
        /// 获取或设置评论 ID。
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置作者 ID。
        /// </summary>
        public int AuthorId { get; set; }
        
        /// <summary>
        /// 获取或设置评论的创建时间。
        /// </summary>
        public DateTime CreationTime { get; set; }
        
        /// <summary>
        /// 获取或设置评论正文实体对象在 MongoDB 中的 ID。
        /// </summary>
        public byte[] ContentId { get; set; }

        /// <summary>
        /// 对于一级评论，获取或设置评论的帖子 ID。
        /// </summary>
        public int? PostId { get; set; }
        
        /// <summary>
        /// 对于二级或更深级别评论，获取或设置父评论 ID。
        /// </summary>
        public int? CommentId { get; set; }
        
        /// <summary>
        /// 获取当前评论的子评论数量。
        /// </summary>
        public int NumberOfComments { get; set; }
        
        /// <summary>
        /// 获取或设置评论是否被删除。
        /// </summary>
        public bool IsRemoved { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置评论的作者。
        /// </summary>
        public virtual UserEntity Author { get; set; }

        /// <summary>
        /// 配置 <see cref="CommentEntity"/> 的数据库模型。
        /// </summary>
        /// <param name="builder"></param>
        public static void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            // 配置自增主键
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.Id)
                   .ValueGeneratedOnAdd();

            // 配置与评论者的一对多关系
            builder.HasOne(entity => entity.Author)
                   .WithMany(entity => entity.Comments)
                   .HasForeignKey(entity => entity.AuthorId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            // 为避免多重级联路径，不在数据库端配置评论与帖子、父评论的一对多关系
            // 这两个一对多关系将在应用层维护
            // 分别配置 PostId，CommentId 字段上的索引
            builder.HasIndex(entity => entity.PostId);
            builder.HasIndex(entity => entity.CommentId);

            // 配置 IsRemoved 字段上的索引
            builder.HasIndex(entity => entity.IsRemoved);
        }
    }
}
