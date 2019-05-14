using System;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Contexts.DependencyInjection;
using BITTreeHole.Data.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 提供数据层的外观。
    /// </summary>
    public interface IDataFacade
    {
        /// <summary>
        /// 获取用户数据集。
        /// </summary>
        IQueryable<UserEntity> Users { get; }
        
        /// <summary>
        /// 获取帖子板块数据集。
        /// </summary>
        IQueryable<PostRegionEntity> PostRegions { get; }
        
        /// <summary>
        /// 获取帖子数据集。
        /// </summary>
        IQueryable<PostEntity> Posts { get; }
        
        /// <summary>
        /// 获取评论数据集。
        /// </summary>
        IQueryable<CommentEntity> Comments { get; }
        
        /// <summary>
        /// 获取用户点赞数据集。
        /// </summary>
        IQueryable<UserVotePostEntity> UserVotePosts { get; }
        
        /// <summary>
        /// 获取用户关注数据集。
        /// </summary>
        IQueryable<UserWatchPostEntity> UserWatchPosts { get; }

        /// <summary>
        /// 添加用户实体对象到数据集中。
        /// </summary>
        /// <param name="user">要添加的用户实体对象。</param>
        /// <exception cref="ArgumentNullException"></exception>
        void AddUser(UserEntity user);

        /// <summary>
        /// 将用户实体对象从数据集中删除。
        /// </summary>
        /// <param name="user">要移除的用户实体对象。</param>
        /// <exception cref="ArgumentNullException"></exception>
        void RemoveUser(UserEntity user);

        /// <summary>
        /// 将所有未提交的更改提交到数据源。
        /// </summary>
        /// <exception cref="DataFacadeException"></exception>
        Task CommitChanges();
    }
    
    namespace DependencyInjection
    {
        /// <summary>
        /// 为 <see cref="DefaultDataFacade"/> 提供依赖注入过程。
        /// </summary>
        internal static class DefaultDataFacadeExtensions
        {
            /// <summary>
            /// 使用默认实现向给定的依赖服务集中添加 <see cref="IDataFacade"/> 依赖项。
            /// </summary>
            /// <param name="services">依赖服务集。</param>
            /// <param name="mysqlConnectionString">MySQL数据库连接字符串。</param>
            /// <param name="mongoConnectionString">MongoDB数据库连接字符串。</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">
            ///     <paramref name="services"/>为null
            ///     或
            ///     <paramref name="mysqlConnectionString"/>为null
            ///     或
            ///     <paramref name="mongoConnectionString"/>为null
            /// </exception>
            public static IServiceCollection AddDefaultDataFacade(this IServiceCollection services, 
                                                                  string mysqlConnectionString,
                                                                  string mongoConnectionString)
            {
                if (services == null)
                    throw new ArgumentNullException(nameof(services));
                if (mysqlConnectionString == null)
                    throw new ArgumentNullException(nameof(mysqlConnectionString));
                if (mongoConnectionString == null)
                    throw new ArgumentNullException(nameof(mongoConnectionString));
            
                // mongoConnectionString 参数暂时未用
                // TODO: 添加 MongoDB 依赖后更新此方法

                services.AddMysqlDbContext(mysqlConnectionString);

                return services;
            }
        }
    }
}
