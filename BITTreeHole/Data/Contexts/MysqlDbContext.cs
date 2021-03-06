using System;
using BITTreeHole.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BITTreeHole.Data.Contexts
{
    /// <summary>
    /// 提供MySQL数据库上下文。
    /// </summary>
    /// <remarks>
    /// 该类仅供数据持久化层内部使用，上层代码不应直接依赖该类的实现。
    /// </remarks>
    public class MysqlDbContext : DbContext
    {
        /// <summary>
        /// 初始化 <see cref="MysqlDbContext"/> 类的新实例。
        /// </summary>
        /// <param name="options">数据库上下文选项。</param>
        public MysqlDbContext(DbContextOptions options)
            : base(options)
        { }
        
        /// <summary>
        /// 获取或设置用户数据集。
        /// </summary>
        public virtual DbSet<UserEntity> Users { get; set; }

        /// <summary>
        /// 获取或设置帖子板块数据集。
        /// </summary>
        public virtual DbSet<PostRegionEntity> PostRegions { get; set; }
        
        /// <summary>
        /// 获取或设置帖子数据集。
        /// </summary>
        public virtual DbSet<PostEntity> Posts { get; set; }
        
        /// <summary>
        /// 获取或设置承载用户关注帖子的多对多关系表。
        /// </summary>
        public virtual DbSet<UserWatchPostEntity> UserWatchPosts { get; set; }
        
        /// <summary>
        /// 获取或设置承载用户点赞帖子的多对多关系表。
        /// </summary>
        public virtual DbSet<UserVotePostEntity> UserVotePosts { get; set; }
        
        /// <summary>
        /// 获取或设置评论数据集。
        /// </summary>
        public virtual DbSet<CommentEntity> Comments { get; set; }

        /// <summary>
        /// 当创建实体对象模型时被调用。
        /// </summary>
        /// <param name="modelBuilder">实体对象模型构建器。</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            UserEntity.Configure(modelBuilder.Entity<UserEntity>());
            PostRegionEntity.Configure(modelBuilder.Entity<PostRegionEntity>());
            PostEntity.Configure(modelBuilder.Entity<PostEntity>());
            UserWatchPostEntity.Configure(modelBuilder.Entity<UserWatchPostEntity>());
            UserVotePostEntity.Configure(modelBuilder.Entity<UserVotePostEntity>());
            CommentEntity.Configure(modelBuilder.Entity<CommentEntity>());
        }
    }

    namespace DependencyInjection
    {
        /// <summary>
        /// 为 <see cref="MysqlDbContext"/> 提供依赖注入过程。
        /// </summary>
        public static class MysqlContextExtensions
        {
            /// <summary>
            /// 向依赖服务集中添加 <see cref="MysqlDbContext"/> 依赖项。
            /// </summary>
            /// <param name="services">依赖服务集。</param>
            /// <param name="connectionString">数据库连接字符串。</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">
            ///     <paramref name="services"/>为null
            ///     或
            ///     <paramref name="connectionString"/>为null。
            /// </exception>
            public static IServiceCollection AddMysqlDbContext(this IServiceCollection services, string connectionString)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                if (connectionString == null)
                    throw new ArgumentNullException(nameof(connectionString));

                return services.AddDbContext<MysqlDbContext>(
                    options => options.UseLazyLoadingProxies()
                                      .UseMySQL(connectionString));
            }
        }
    }
}
