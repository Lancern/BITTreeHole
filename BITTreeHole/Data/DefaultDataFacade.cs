using System;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Contexts;
using BITTreeHole.Data.Entities;
using MySql.Data.MySqlClient;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 提供 <see cref="IDataFacade"/> 的默认实现。
    /// </summary>
    internal sealed class DefaultDataFacade : IDataFacade
    {
        private readonly MysqlDbContext _mysqlDbContext;
        private readonly MongoDbContext _mongoDbContext;

        /// <summary>
        /// 初始化 <see cref="DefaultDataFacade"/> 类的新实例。
        /// <see cref="DefaultDataFacade"/> 类的初始化将会创建 MySQL 数据库结构。
        /// </summary>
        /// <param name="mysqlDbContext">MySQL数据上下文。</param>
        /// <param name="mongoDbContext">MongoDB数据上下文。</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="mysqlDbContext"/>为null
        ///     或
        ///     <paramref name="mongoDbContext"/>为null
        /// </exception>
        public DefaultDataFacade(MysqlDbContext mysqlDbContext, MongoDbContext mongoDbContext)
        {
            _mysqlDbContext = mysqlDbContext ?? throw new ArgumentNullException(nameof(mysqlDbContext));
            _mongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));

            // TODO: 尝试重构下面的代码以分离数据库创建检查逻辑
            _mysqlDbContext.Database.EnsureCreated();
        }

        /// <inheritdoc />
        public IQueryable<UserEntity> Users => _mysqlDbContext.Users;

        /// <inheritdoc />
        public IQueryable<PostRegionEntity> PostRegions => _mysqlDbContext.PostRegions;

        /// <inheritdoc />
        public IQueryable<PostEntity> Posts => _mysqlDbContext.Posts;

        /// <inheritdoc />
        public IQueryable<CommentEntity> Comments => _mysqlDbContext.Comments;

        /// <inheritdoc />
        public IQueryable<UserVotePostEntity> UserVotePosts => _mysqlDbContext.UserVotePosts;

        /// <inheritdoc />
        public IQueryable<UserWatchPostEntity> UserWatchPosts => _mysqlDbContext.UserWatchPosts;

        /// <inheritdoc />
        public void AddUser(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            _mysqlDbContext.Users.AddAsync(user);
        }

        /// <inheritdoc />
        public void RemoveUser(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            _mysqlDbContext.Users.Remove(user);
        }

        /// <inheritdoc />
        public void AddPostRegion(PostRegionEntity postRegion)
        {
            if (postRegion == null)
                throw new ArgumentNullException(nameof(postRegion));
            _mysqlDbContext.PostRegions.Add(postRegion);
        }

        /// <inheritdoc />
        public void RemovePostRegion(PostRegionEntity postRegion)
        {
            if (postRegion == null)
                throw new ArgumentNullException(nameof(postRegion));
            _mysqlDbContext.PostRegions.Remove(postRegion);
        }

        /// <inheritdoc />
        public async Task CommitChanges()
        {
            try
            {
                await _mysqlDbContext.SaveChangesAsync();
            }
            catch (MySqlException ex)
            {
                throw new DataFacadeException("MySQL数据源抛出了未经处理的异常。", ex);
            }
        }
    }
}
