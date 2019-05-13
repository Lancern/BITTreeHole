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
        /// 获取或设置用户的用户名。
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// 获取或设置用户性别。false：女性，true：男性。
        /// </summary>
        public bool? Gender { get; set; }

        /// <summary>
        /// 获取当前用户实体对象是否处于初始状态（即用户第一次登录后的状态）。
        /// </summary>
        public bool IsFresh => Gender == null;

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
            
            // 配置 Username 属性上的长度约束。
            builder.Property(entity => entity.Username)
                   .HasMaxLength(20);
            
            // 配置 Username 属性上的 unique 索引。
            builder.HasIndex(entity => entity.Username)
                   .IsUnique();

            // 在实体对象模型中移除计算属性 IsFresh
            builder.Ignore(entity => entity.IsFresh);
        }
    }
}
