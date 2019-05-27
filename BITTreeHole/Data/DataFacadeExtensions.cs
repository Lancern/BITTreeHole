using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BITTreeHole.Data.Entities;
using BITTreeHole.Extensions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace BITTreeHole.Data
{
    /// <summary>
    /// 为 <see cref="IDataFacade"/> 提供扩展方法。
    /// </summary>
    public static class DataFacadeExtensions
    {
        /// <summary>
        /// 当给定的用户不存在时，向数据源添加用户。返回用户实体对象。
        /// </summary>
        /// <param name="dataFacade">数据源外观。</param>
        /// <param name="wechatId">用户的微信 ID。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dataFacade"/> 为 null
        ///     或
        ///     <paramref name="wechatId"/> 为 null
        /// </exception>
        public static async Task<UserEntity> AddOrFindUserByWechatId(this IDataFacade dataFacade, string wechatId)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (wechatId == null)
                throw new ArgumentNullException(nameof(wechatId));

            var existingEntity = await dataFacade.Users
                                                 .Where(u => u.WechatId == wechatId)
                                                 .FirstOrDefaultAsync();
            if (existingEntity != null)
            {
                // 用户已经存在于数据源中
                return existingEntity;
            }
            
            // 用户在数据源中未找到
            var userEntity = new UserEntity
            {
                WechatId = wechatId
            };

            try
            {
                dataFacade.AddUser(userEntity);
                await dataFacade.CommitChanges();
                return userEntity;
            }
            catch (DataFacadeException)
            {
                // 重做用户查询
                existingEntity = await dataFacade.Users
                                                 .Where(u => u.WechatId == wechatId)
                                                 .FirstOrDefaultAsync();
                if (existingEntity != null)
                {
                    return existingEntity;
                }

                // 重做用户查询仍然无法找到用户
                throw;
            }
        }

        /// <summary>
        /// 向数据源中添加帖子。
        /// </summary>
        /// <param name="dataFacade">数据源的外观。</param>
        /// <param name="indexEntity">帖子索引实体对象。</param>
        /// <param name="contentEntity">帖子内容对象。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dataFacade"/>为null
        ///     或
        ///     <paramref name="indexEntity"/>为null
        ///     或
        ///     <paramref name="contentEntity"/>为null
        /// </exception>
        /// <exception cref="DataFacadeException">数据源抛出了未经处理的异常</exception>
        public static async Task<int> AddPost(this IDataFacade dataFacade,
                                              PostEntity indexEntity, PostContentEntity contentEntity)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (indexEntity == null)
                throw new ArgumentNullException(nameof(indexEntity));
            if (contentEntity == null)
                throw new ArgumentNullException(nameof(contentEntity));
            
            // 将内容实体对象插入到数据源中。
            await dataFacade.AddPostContentEntity(contentEntity);
            
            // 将帖子索引实体对象插入到数据源中。
            dataFacade.AddPostIndexEntity(indexEntity);
            await dataFacade.CommitChanges();

            return indexEntity.Id;
        }

        /// <summary>
        /// 通过帖子 ID 获取帖子数据。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="postId">帖子 ID。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="dataFacade"/>为null</exception>
        /// <exception cref="PostNotFoundException">指定的帖子未找到</exception>
        public static async Task<(PostEntity IndexEntity, PostContentEntity ContentEntity)>
            FindPost(this IDataFacade dataFacade, int postId)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));

            var indexEntity = await dataFacade.Posts
                                              // 下面的语句中务必使用 e.IsRemoved == false 以正确引导 EFCore 解析查询
                                              .FirstOrDefaultAsync(e => e.Id == postId && e.IsRemoved == false);
            if (indexEntity == null)
            {
                // 没有在数据源中找到指定的帖子 ID
                throw new PostNotFoundException();
            }

            var contentId = new ObjectId(indexEntity.ContentId);
            var contentEntity = await dataFacade.FindPostContentEntity(contentId);

            return (indexEntity, contentEntity);
        }

        /// <summary>
        /// 查询帖子信息。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="region">帖子所属板块 ID。</param>
        /// <param name="page">分页参数，页面编号。页面编号从 0 开始。</param>
        /// <param name="itemsPerPage">分页参数，每一页上的帖子数量。</param>
        /// <returns>帖子信息实体对象与帖子正文实体对象二元组集合</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dataFacade"/>为null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="page"/>小于零
        ///     或
        ///     <paramref name="itemsPerPage"/>小于等于零
        /// </exception>
        public static async Task<IEnumerable<(PostEntity IndexEntity, PostContentEntity ContentEntity)>>
            FindPosts(this IDataFacade dataFacade, int region, int page, int itemsPerPage)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (page < 0)
                throw new ArgumentOutOfRangeException(nameof(page));
            if (itemsPerPage <= 0)
                throw new ArgumentOutOfRangeException(nameof(itemsPerPage));
            
            var indexEntities = await dataFacade.Posts
                                                .AsNoTracking()
                                                // 下面的语句中务必使用 e.IsRemoved == false 以正确引导 EFCore 解析查询
                                                .Where(e => e.PostRegionId == region && e.IsRemoved == false)
                                                .OrderByDescending(e => e.UpdateTime)
                                                .Paginate(page, itemsPerPage)
                                                .ToListAsync();
            var contentEntities =
                await dataFacade.FindPostContentEntities(indexEntities.Select(e => new ObjectId(e.ContentId)));
            var contentDict = contentEntities.ToDictionary(e => e.Id);

            return indexEntities.Select(indexEntity =>
            {
                if (contentDict.TryGetValue(new ObjectId(indexEntity.ContentId), out var contentEntity))
                    return (indexEntity, contentEntity);
                return (indexEntity, null);
            });
        }

        /// <summary>
        /// 获取指定帖子的创建者 ID。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="postId">帖子 ID。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataFacade"/>为null
        /// </exception>
        /// <exception cref="PostNotFoundException">指定的帖子不存在</exception>
        public static async Task<int> GetPostAuthorId(this IDataFacade dataFacade, int postId)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));

            var indexEntity = await dataFacade.Posts
                                              .AsNoTracking()
                                              .Where(e => e.Id == postId && e.IsRemoved == false)
                                              .FirstOrDefaultAsync();
            if (indexEntity == null)
            {
                throw new PostNotFoundException();
            }

            return indexEntity.AuthorId;
        }

        /// <summary>
        /// 向给定的帖子上传图片。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="postId">帖子 ID。</param>
        /// <param name="images">图片 ID 及对应图片的数据流工厂。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="images"/>为null
        /// </exception>
        /// <exception cref="PostNotFoundException">
        ///     指定的帖子不存在
        ///     或
        ///     帖子存在但其删除标记为true
        /// </exception>
        public static async Task UpdatePostImages(this IDataFacade dataFacade, 
                                                  int postId, IReadOnlyDictionary<int, Func<Stream>> images)
        {
            if (images == null)
                throw new ArgumentNullException(nameof(images));
            
            // TODO: 在这里添加代码对 images 执行验证

            var indexEntity = await dataFacade.Posts
                                              // 下面的语句中务必使用 e.IsRemoved == true 以正确引导 EF Core 建立查询
                                              .Where(e => e.Id == postId && e.IsRemoved == false)
                                              .FirstOrDefaultAsync();
            if (indexEntity == null)
            {
                // 指定的帖子不存在
                throw new PostNotFoundException();
            }
            
            var contentId = new ObjectId(indexEntity.ContentId);
            var contentEntity = await dataFacade.FindPostContentEntity(contentId);
            
            // 删除已有的、被覆盖的图片数据
            var removeTasks = new List<Task>();
            foreach (var imageIndex in images.Keys.Where(i => i >= 0 && i < contentEntity.ImageIds.Length))
            {
                var imageId = contentEntity.ImageIds[imageIndex];
                if (imageId == null)
                {
                    continue;
                }
                
                removeTasks.Add(dataFacade.RemoveImage(imageId.Value));
            }

            await Task.WhenAll(removeTasks);

            // 将图片上传至数据源
            var imageIds = new ConcurrentDictionary<int, ObjectId?>();
            var uploadTasks = new List<Task>();
            foreach (var (index, imageDataStreamFactory) in images)
            {
                using (var imageDataStream = imageDataStreamFactory())
                {
                    uploadTasks.Add(dataFacade.UploadImage(imageDataStream)
                                                 .ContinueWith((t, i) => imageIds.TryAdd((int)i, t.Result), index));
                }
            }

            await Task.WhenAll(uploadTasks);

            // 更新内容实体对象
            await dataFacade.UpdatePostContentImageIds(contentId, imageIds);
            
            // 更新索引实体对象中的上次更新时间戳
            indexEntity.UpdateTime = DateTime.Now;
            await dataFacade.CommitChanges();
        }

        /// <summary>
        /// 移除帖子图片。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="postId">帖子 ID。</param>
        /// <param name="indexesToRemove">要移除的图片索引。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dataFacade"/>为null
        ///     或
        ///     <paramref name="indexesToRemove"/>为null
        /// </exception>
        /// <exception cref="PostNotFoundException">指定的帖子不存在。</exception>
        public static async Task RemovePostImages(this IDataFacade dataFacade,
                                                  int postId, IEnumerable<int> indexesToRemove)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (indexesToRemove == null)
                throw new ArgumentNullException(nameof(indexesToRemove));
            
            // TODO: 在这里添加代码对 indexesToRemove 进行验证

            var indexEntity = await dataFacade.Posts
                                              .Where(e => e.Id == postId)
                                              .FirstOrDefaultAsync();
            if (indexEntity == null)
            {
                throw new PostNotFoundException();
            }
            
            var contentId = new ObjectId(indexEntity.ContentId);
            var contentEntity = await dataFacade.FindPostContentEntity(contentId);

            var indexesToRemoveConcrete = indexesToRemove.ToArray();
            
            // 删除图片数据
            var removeTasks = new List<Task>();
            foreach (var imageIndex in indexesToRemoveConcrete.Where(i => i >= 0 && i < contentEntity.ImageIds.Length))
            {
                var imageId = contentEntity.ImageIds[imageIndex];
                if (imageId == null)
                {
                    continue;
                }
                
                removeTasks.Add(dataFacade.RemoveImage(imageId.Value));
            }

            await Task.WhenAll(removeTasks);

            // 更新内容实体对象
            var imageSetDict = indexesToRemoveConcrete.ToDictionary<int, int, ObjectId?>(i => i, i => null);
            await dataFacade.UpdatePostContentImageIds(contentId, imageSetDict);
            
            // 更新索引实体对象中的最后更新时间戳
            indexEntity.UpdateTime = DateTime.Now;
            await dataFacade.CommitChanges();
        }
        
        /// <summary>
        /// 删除指定的帖子。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="postId">要删除的帖子 ID。</param>
        /// <param name="permanent">是否永久性地删除帖子数据</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataFacade"/>为null
        /// </exception>
        /// <exception cref="DataFacadeException">当数据源抛出了未经处理的异常时抛出</exception>
        public static async Task RemovePost(this IDataFacade dataFacade, int postId, bool permanent = false)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));
            if (permanent)
                throw new NotImplementedException();

            var indexEntity = await dataFacade.Posts
                                              .Where(e => e.Id == postId && e.IsRemoved == false)
                                              .FirstOrDefaultAsync();
            if (indexEntity == null)
            {
                throw new PostNotFoundException();
            }

            indexEntity.IsRemoved = true;
            await dataFacade.CommitChanges();
        }

        /// <summary>
        /// 向数据源添加评论索引实体对象及其对应的内容实体对象。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="indexEntity">要添加的索引实体对象。</param>
        /// <param name="contentEntity">要添加的内容实体对象。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="indexEntity"/>为null
        ///     或
        ///     <paramref name="contentEntity"/>为null
        /// </exception>
        public static async Task AddPostComment(this IDataFacade dataFacade,
                                                CommentEntity indexEntity, CommentContentEntity contentEntity)
        {
            if (indexEntity == null)
                throw new ArgumentNullException(nameof(indexEntity));
            if (contentEntity == null)
                throw new ArgumentNullException(nameof(contentEntity));

            dataFacade.AddCommentIndexEntity(indexEntity);
            await dataFacade.CommitChanges();

            await dataFacade.AddCommentContentEntity(contentEntity);
        }

        /// <summary>
        /// 获取帖子的评论列表。
        /// </summary>
        /// <param name="dataFacade"></param>
        /// <param name="postId">帖子 ID</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="dataFacade"/>为null</exception>
        public static async Task<List<(CommentEntity IndexEntity, CommentContentEntity ContentEntity)>>
            FindPostComments(this IDataFacade dataFacade, int postId)
        {
            if (dataFacade == null)
                throw new ArgumentNullException(nameof(dataFacade));

            // 获取所有一级评论
            var indexEntitiesLv1 = await dataFacade.Comments
                                                   .Where(e => e.PostId == postId && e.IsRemoved == false)
                                                   .ToListAsync();
            var contentEntitiesLv1 = await dataFacade.FindCommentContentEntities(
                indexEntitiesLv1.Select(e => new ObjectId(e.ContentId)));

            var indexEntitiesLv2 = new List<CommentEntity>();
            var contentEntitiesLv2 = new List<CommentContentEntity>();
            foreach (var entityLv1 in indexEntitiesLv1)
            {
                // 获取当前枚举到的一级评论下的所有二级评论
                var indexEntitiesLv2Current = await dataFacade.Comments
                                                              .Where(e => e.CommentId == entityLv1.Id &&
                                                                          e.IsRemoved == false)
                                                              .ToListAsync();
                var contentEntitiesLv2Current = await dataFacade.FindCommentContentEntities(
                    indexEntitiesLv2.Select(e => new ObjectId(e.ContentId)));
                
                indexEntitiesLv2.AddRange(indexEntitiesLv2Current);
                contentEntitiesLv2.AddRange(contentEntitiesLv2Current);
            }

            // 将一级评论与二级评论进行扁平化处理
            var indexEntities = indexEntitiesLv1;
            indexEntities.AddRange(indexEntitiesLv2);

            var contentEntities = contentEntitiesLv1;
            contentEntitiesLv1.AddRange(contentEntitiesLv2);

            return indexEntities.Zip(contentEntities, (ie, ce) => (ie, ce))
                                .ToList();
        }
    }
}
