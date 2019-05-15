using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BITTreeHole.Data.Entities
{
    /// <summary>
    /// 表示帖子板块实体对象。
    /// </summary>
    public class PostRegionEntity
    {
        /// <summary>
        /// 获取或设置板块 ID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 获取或设置板块标题。
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 获取或设置板块的图标数据。
        /// </summary>
        public byte[] IconData { get; set; }
        
        /// <summary>
        /// 导航属性，获取或设置当前板块下拥有的帖子。
        /// </summary>
        public virtual ICollection<PostEntity> Posts { get; set; }

        /// <summary>
        /// 配置板块实体对象的数据库模型。
        /// </summary>
        /// <param name="builder">实体对象的数据库模型的构造器。</param>
        public static void Configure(EntityTypeBuilder<PostRegionEntity> builder)
        {
            // 配置自增主键字段 ID
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.Id)
                   .ValueGeneratedOnAdd();

            // 配置标题字段
            builder.Property(entity => entity.Title)
                   .HasMaxLength(4);
            
            // 配置标题字段上的唯一性索引
            builder.HasIndex(entity => entity.Title)
                   .IsUnique();
        }
    }
}
