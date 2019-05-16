using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Contexts.DependencyInjection;
using BITTreeHole.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

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
        /// <exception cref="ArgumentNullException"><paramref name="user"/>为null</exception>
        void AddUser(UserEntity user);

        /// <summary>
        /// 将用户实体对象从数据集中删除。
        /// </summary>
        /// <param name="user">要移除的用户实体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="user"/>为null</exception>
        void RemoveUser(UserEntity user);
        
        /// <summary>
        /// 添加帖子板块。
        /// </summary>
        /// <param name="postRegion">帖子板块实体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="postRegion"/>为null</exception>
        void AddPostRegion(PostRegionEntity postRegion);

        /// <summary>
        /// 移除帖子模块。
        /// </summary>
        /// <param name="postRegion">帖子模块实体对象。</param>
        /// <exception cref="ArgumentNullException"><paramref name="postRegion"/>为null</exception>
        void RemovePostRegion(PostRegionEntity postRegion);

        /// <summary>
        /// 查询帖子正文实体对象。
        /// </summary>
        /// <param name="contentIds">需要查询的帖子正文实体对象的 ID。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="contentIds"/>为null</exception>
        /// <exception cref="DataFacadeException">当数据源抛出了未经处理的异常时</exception>
        Task<List<PostContentEntity>> FindPostContentEntities(IEnumerable<ObjectId> contentIds);

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

                services.AddMysqlDbContext(mysqlConnectionString);
                services.AddMongoDbContext(mongoConnectionString);
                services.AddScoped<IDataFacade, DefaultDataFacade>();

                return services;
            }
        }
    }
}
