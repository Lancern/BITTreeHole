using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Contexts;
using BITTreeHole.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

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

        /// <summary>
        /// 执行给定的异步回调并将数据源抛出的异常包装至 <see cref="DataFacadeException"/> 中抛出。
        /// </summary>
        /// <param name="actions">要执行的异步回调。</param>
        /// <returns></returns>
        /// <exception cref="DataFacadeException">当异步回调抛出了数据源相关异常时抛出。</exception>
        private async Task AccessDataSource(Func<Task> actions)
        {
            try
            {
                await actions();
            }
            catch (MongoException ex)
            {
                throw new DataFacadeException("MongoDB数据源抛出了未经处理的异常。", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new DataFacadeException("MySQL数据源抛出了未经处理的异常。", ex);
            }
        }

        /// <summary>
        /// 执行给定的异步回调并将数据源抛出的异常包装至 <see cref="DataFacadeException"/> 中抛出。
        /// </summary>
        /// <param name="actions">要执行的异步回调。</param>
        /// <returns>异步回调的返回值。</returns>
        /// <exception cref="DataFacadeException">当异步回调抛出了数据源相关异常时抛出。</exception>
        private async Task<T> AccessDataSource<T>(Func<Task<T>> actions)
        {
            try
            {
                return await actions();
            }
            catch (MongoException ex)
            {
                throw new DataFacadeException("MongoDB数据源抛出了未经处理的异常。", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new DataFacadeException("MySQL数据源抛出了未经处理的异常。", ex);
            }
        }

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
        public void AddPostIndexEntity(PostEntity postIndexEntity)
        {
            if (postIndexEntity == null)
                throw new ArgumentNullException(nameof(postIndexEntity));
            _mysqlDbContext.Posts.Add(postIndexEntity);
        }

        /// <inheritdoc />
        public async Task AddPostContentEntity(PostContentEntity postContentEntity)
        {
            if (postContentEntity == null)
                throw new ArgumentNullException(nameof(postContentEntity));

            await AccessDataSource(async () => await _mongoDbContext.PostContents.InsertOneAsync(postContentEntity));
        }

        /// <inheritdoc />
        public async Task<PostContentEntity> FindPostContentEntity(ObjectId contentId)
        {
            return await AccessDataSource(
                async () => await _mongoDbContext.PostContents
                                                 .Find(Builders<PostContentEntity>.Filter.Eq(e => e.Id, contentId))
                                                 .FirstOrDefaultAsync());
        }

        /// <inheritdoc />
        public async Task<List<PostContentEntity>> FindPostContentEntities(IEnumerable<ObjectId> contentIds)
        {
            if (contentIds == null)
                throw new ArgumentNullException(nameof(contentIds));

            return await AccessDataSource(
                async () => await _mongoDbContext.PostContents
                                                 .Find(Builders<PostContentEntity>.Filter.In(e => e.Id, contentIds))
                                                 .ToListAsync());
        }

        /// <inheritdoc />
        public async Task UpdatePostContentText(ObjectId postContentId, string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            await AccessDataSource(
                async () => await _mongoDbContext.PostContents
                                                 .UpdateOneAsync(Builders<PostContentEntity>.Filter.Eq(e => e.Id, postContentId),
                                                     Builders<PostContentEntity>.Update.Set(e => e.Text, text)));
        }

        /// <inheritdoc />
        public async Task UpdatePostContentImageIds(ObjectId postContentId, IReadOnlyDictionary<int, ObjectId?> positionValue)
        {
            if (positionValue == null)
                throw new ArgumentNullException(nameof(positionValue));
            
            // TODO: 在这里添加代码对 positionValue 进行验证

            var imageIds = await AccessDataSource(
                async () => await _mongoDbContext.PostContents
                                                 .Find(Builders<PostContentEntity>.Filter.Eq(e => e.Id, postContentId))
                                                 .Project(e => e.ImageIds)
                                                 .FirstOrDefaultAsync());

            var maxIndex = positionValue.Keys.Max();
            if (imageIds.Length <= maxIndex)
            {
                var newImageIds = new ObjectId?[maxIndex + 1];
                Array.Copy(imageIds, newImageIds, imageIds.Length);
                imageIds = newImageIds;
            }

            foreach (var (index, id) in positionValue)
            {
                imageIds[index] = id;
            }

            await AccessDataSource(
                async () => await _mongoDbContext.PostContents
                                                 .UpdateOneAsync(
                                                     Builders<PostContentEntity>.Filter.Eq(e => e.Id, postContentId),
                                                     Builders<PostContentEntity>
                                                         .Update.Set(e => e.ImageIds, imageIds)));
        }

        /// <inheritdoc />
        public async Task<ObjectId> UploadImage(Stream imageDataStream)
        {
            if (imageDataStream == null)
                throw new ArgumentNullException(nameof(imageDataStream));

            return await AccessDataSource(
                async () => await _mongoDbContext.ImageBucket.UploadFromStreamAsync(string.Empty, imageDataStream));
        }

        /// <inheritdoc />
        public async Task RemoveImage(ObjectId imageId)
        {
            await AccessDataSource(
                async () => await _mongoDbContext.ImageBucket.DeleteAsync(imageId));
        }

        /// <inheritdoc />
        public async Task CommitChanges()
        {
            await AccessDataSource(async () => await _mysqlDbContext.SaveChangesAsync());
        }
    }
}
